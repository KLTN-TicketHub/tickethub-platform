using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Repositories
{
    public class IssuedTicketRepository : BaseRepository<IssuedTicket, InventoryDbContext>, IIssuedTicketRepository
    {
        public IssuedTicketRepository(InventoryDbContext context) : base(context)
        {
        }
    }
}
