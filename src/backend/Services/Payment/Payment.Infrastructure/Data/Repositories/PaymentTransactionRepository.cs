using BuildingBlocks.Infrastructure.Data;
using Payment.Infrastructure.Data.Contexts;
using Payment.Infrastructure.Entities;
using Payment.Infrastructure.Interfaces.IRepositories;

namespace Payment.Infrastructure.Data.Repositories
{
    public class PaymentTransactionRepository : BaseRepository<PaymentTransaction, PaymentDbContext>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(PaymentDbContext dbContext) : base(dbContext)
        {
        }
    }
}
