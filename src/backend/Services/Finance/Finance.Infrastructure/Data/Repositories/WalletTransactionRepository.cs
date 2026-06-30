using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction, FinanceDbContext>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
