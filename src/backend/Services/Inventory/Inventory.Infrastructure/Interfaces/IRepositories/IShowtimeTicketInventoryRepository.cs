using BuildingBlocks.Domain.DDD;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Entities;

namespace Inventory.Infrastructure.Interfaces.IRepositories
{
    public interface IShowtimeTicketInventoryRepository : IBaseRepository<ShowtimeTicketInventory, InventoryDbContext>
    {
    }
}
