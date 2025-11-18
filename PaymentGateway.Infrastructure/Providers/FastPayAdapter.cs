using PaymentGateway.Application.Ports;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Infrastructure.Config;

namespace PaymentGateway.Infrastructure.Providers
{
    public class FastPayAdapter : IProviderPort
    {
        public string Name => ProvidersNamesEnum.FastPay.ToString();
        public bool IsAvailable { get; private set; }
        private readonly Random _rnd = new();

        public FastPayAdapter(ProviderAvailability cfg) => IsAvailable = cfg.FastPayAvailable;

        public decimal CalculateFee(decimal amount) => Math.Round(amount * 0.0349m, 2);

        public async Task<ProviderResponse> ProcessPaymentAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            if (!IsAvailable) throw new InvalidOperationException($"{Name} unavailable");

            var payload = new
            {
                transaction_amount = amount,
                currency = currency,
                payer = new { email = "cliente@teste.com" },
                installments = 1,
                description = $"Compra via {Name}"
            };

            await Task.Delay(80, ct);

            var externalId = new ExternalId($"FP-{_rnd.Next(100000, 999999)}");
            bool approved = true;

            return new ProviderResponse(externalId, approved);
        }
    }

}
