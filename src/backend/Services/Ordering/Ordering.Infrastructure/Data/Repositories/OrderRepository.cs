using BuildingBlocks.Infrastructure.Data;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces.IRepositories;

namespace Ordering.Infrastructure.Data.Repositories
{
    public class OrderRepository : BaseRepository<Order, OrderingDbContext>, IOrderRepository
    {
        public OrderRepository(OrderingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
