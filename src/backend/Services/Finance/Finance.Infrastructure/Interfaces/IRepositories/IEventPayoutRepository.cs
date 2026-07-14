using BuildingBlocks.Domain.DDD;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;

namespace Finance.Infrastructure.Interfaces.IRepositories
{
    public interface IEventPayoutRepository : IBaseRepository<EventPayout, FinanceDbContext>
    {
    }
}
