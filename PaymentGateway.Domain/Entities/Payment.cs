using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Money Gross { get; private set; }
        public decimal Fee { get; private set; }
        public Money Net { get; private set; }
        public ExternalId ExternalId { get; private set; }
        public PaymentStatusEnum Status { get; private set; }
        public string ProviderName { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        public Payment(Money gross)
        {
            Id = Guid.NewGuid();
            Gross = gross ?? throw new ArgumentNullException(nameof(gross));
            Status = PaymentStatusEnum.Pending;
            ExternalId = ExternalId.None;
            Fee = 0m;
            Net = new Money(gross.Amount, gross.Currency);
            CreatedAt = DateTime.UtcNow;
        }

        public void ApplyProviderResult(string providerName, ExternalId externalId, bool approved, decimal fee)
        {
            ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
            ExternalId = externalId ?? ExternalId.None;
            Fee = decimal.Round(fee, 2);
            Net = Gross.Subtract(Fee);
            Status = approved ? PaymentStatusEnum.Approved : PaymentStatusEnum.Failed;
        }
    }

}
