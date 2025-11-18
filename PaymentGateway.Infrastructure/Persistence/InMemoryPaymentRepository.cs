using PaymentGateway.Application.Ports;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure.Persistence
{
    public class InMemoryPaymentRepository : IPaymentRepository
    {
        private readonly Dictionary<Guid, Payment> _store = [];

        public Task SaveAsync(Payment payment, CancellationToken ct = default)
        {
            _store[payment.Id] = payment;
            return Task.CompletedTask;
        }

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            _store.TryGetValue(id, out var p);
            return Task.FromResult(p);
        }

        public Task<List<Payment>> GetAllAsync(CancellationToken ct = default)
        {
            var all = _store.Values.ToList();
            return Task.FromResult(all);
        }
    }
}
