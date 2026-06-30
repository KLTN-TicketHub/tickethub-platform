using BuildingBlocks.Domain.DDD;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Interfaces.IRepositories
{
    public interface IOrderRepository : IBaseRepository<Order, OrderingDbContext>
    {
    }
}
