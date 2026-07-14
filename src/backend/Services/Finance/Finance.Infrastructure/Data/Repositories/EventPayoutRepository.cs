using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class EventPayoutRepository : BaseRepository<EventPayout, FinanceDbContext>, IEventPayoutRepository
    {
        public EventPayoutRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
