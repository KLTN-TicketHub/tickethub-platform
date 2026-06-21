using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Repositories
{
    public class ShowtimeSeatRepository : BaseRepository<ShowtimeSeat, InventoryDbContext>, IShowtimeSeatRepository
    {
        public ShowtimeSeatRepository(InventoryDbContext context) : base(context)
        {
        }
    }
}
