using PaymentGateway.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Ports
{
    public interface IPaymentRepository
    {
        Task SaveAsync(Payment payment, CancellationToken ct = default);
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Payment>> GetAllAsync(CancellationToken ct = default);
    }
}
