using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Repositories
{
    public class ShowtimeTicketInventoryRepository : BaseRepository<ShowtimeTicketInventory, InventoryDbContext>, IShowtimeTicketInventoryRepository
    {
        public ShowtimeTicketInventoryRepository(InventoryDbContext context) : base(context)
        {
        }
    }
}
