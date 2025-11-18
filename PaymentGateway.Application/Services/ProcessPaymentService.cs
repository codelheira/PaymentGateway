using PaymentGateway.Application.Ports;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;
using static PaymentGateway.Application.DTOs.PaymentsDTOs;

namespace PaymentGateway.Application.Services
{
    public class ProcessPaymentService : IProcessPaymentUseCase
    {
        private readonly IEnumerable<IProviderPort> _providers;
        private readonly IPaymentRepository _repo;

        public ProcessPaymentService(IEnumerable<IProviderPort> providers, IPaymentRepository repo)
        {
            _providers = providers;
            _repo = repo;
        }

        public async Task<ProcessPaymentResult> ExecuteAsync(ProcessPaymentRequest request, CancellationToken ct = default)
        {
            if (request.Amount <= 0) throw new ArgumentException("Amount must be > 0");
            var money = new Money(request.Amount, request.Currency);

            var payment = new Payment(money);

            string preferredName = request.Amount < 100m ? ProvidersNamesEnum.FastPay.ToString() : ProvidersNamesEnum.SecurePay.ToString();

            var preferred = _providers.FirstOrDefault(p => p.Name == preferredName);
            var fallback = _providers.FirstOrDefault(p => p.Name != preferredName);

            if (preferred == null || fallback == null) throw new InvalidOperationException("Providers not registered");

            IProviderPort usedProvider = preferred;
            ProviderResponse providerResponse;

            if (preferred.IsAvailable)
            {
                try
                {
                    providerResponse = await preferred.ProcessPaymentAsync(request.Amount, request.Currency, ct);
                }
                catch
                {
                    providerResponse = await TryFallbackAsync(fallback, request, ct);
                    usedProvider = fallback;
                }
            }
            else
            {
                providerResponse = await TryFallbackAsync(fallback, request, ct);
                usedProvider = fallback;
            }

            decimal fee = usedProvider.CalculateFee(request.Amount);

            payment.ApplyProviderResult(usedProvider.Name, providerResponse.ExternalId, providerResponse.Approved, fee);

            await _repo.SaveAsync(payment, ct);

            var result = new ProcessPaymentResult(
                payment.Id,
                payment.ExternalId.Value,
                payment.Status.ToString().ToLowerInvariant(),
                payment.ProviderName,
                payment.Gross.Amount,
                payment.Fee,
                payment.Net.Amount
            );

            return result;
        }

        private static async Task<ProviderResponse> TryFallbackAsync(IProviderPort fallback, ProcessPaymentRequest request, CancellationToken ct)
        {
            if (!fallback.IsAvailable) throw new InvalidOperationException("no provider available");
            try
            {
                return await fallback.ProcessPaymentAsync(request.Amount, request.Currency, ct);
            }
            catch
            {
                throw new InvalidOperationException("no provider available");
            }
        }
    }

}
