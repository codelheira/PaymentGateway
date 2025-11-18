using PaymentGateway.Application.Ports;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;
using PaymentGateway.Infrastructure.Config;

namespace PaymentGateway.Infrastructure.Providers
{
    public class SecurePayAdapter : IProviderPort
    {
        public string Name => ProvidersNamesEnum.SecurePay.ToString();
        public bool IsAvailable { get; private set; }
        private readonly Random _rnd = new();

        public SecurePayAdapter(ProviderAvailability cfg) => IsAvailable = cfg.SecurePayAvailable;

        public decimal CalculateFee(decimal amount) => Math.Round(amount * 0.0299m + 0.40m, 2);

        public async Task<ProviderResponse> ProcessPaymentAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            if (!IsAvailable) throw new InvalidOperationException($"{Name} unavailable");

            var payload = new
            {
                amount_cents = (int)Math.Round(amount * 100),
                currency_code = currency,
                client_reference = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            await Task.Delay(100, ct);

            var externalId = new ExternalId($"SP-{_rnd.Next(10000, 99999)}");
            bool approved = true;

            return new ProviderResponse(externalId, approved);
        }

    }
}
