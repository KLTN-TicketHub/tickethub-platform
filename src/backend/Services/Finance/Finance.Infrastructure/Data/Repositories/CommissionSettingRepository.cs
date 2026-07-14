using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class CommissionSettingRepository : BaseRepository<CommissionSetting, FinanceDbContext>, ICommissionSettingRepository
    {
        public CommissionSettingRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
