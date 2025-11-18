using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Ports
{
    public record ProviderResponse(ExternalId ExternalId, bool Approved);

    public interface IProviderPort
    {
        string Name { get; }
        bool IsAvailable { get; }
        decimal CalculateFee(decimal amount);
        Task<ProviderResponse> ProcessPaymentAsync(decimal amount, string currency, CancellationToken ct = default);
    }
}
