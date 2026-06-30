using BuildingBlocks.Domain.DDD;
using Payment.Infrastructure.Data.Contexts;
using Payment.Infrastructure.Entities;

namespace Payment.Infrastructure.Interfaces.IRepositories
{
    public interface IPaymentTransactionRepository : IBaseRepository<PaymentTransaction, PaymentDbContext>
    {
    }
}
