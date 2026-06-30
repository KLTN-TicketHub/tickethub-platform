using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class WalletRepository : BaseRepository<Wallet, FinanceDbContext>, IWalletRepository
    {
        public WalletRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
