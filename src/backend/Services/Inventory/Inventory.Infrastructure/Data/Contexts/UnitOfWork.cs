using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<InventoryDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            InventoryDbContext dbContext,
            IAuditLogRepository auditLogRepository,
            IShowtimeSeatRepository showtimeSeatRepository,
            IShowtimeTicketInventoryRepository showtimeTicketInventoryRepository,
            IIssuedTicketRepository issuedTicketRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
            ShowtimeSeatRepository = showtimeSeatRepository;
            ShowtimeTicketInventoryRepository = showtimeTicketInventoryRepository;
            IssuedTicketRepository = issuedTicketRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
        public IShowtimeSeatRepository ShowtimeSeatRepository { get; }
        public IShowtimeTicketInventoryRepository ShowtimeTicketInventoryRepository { get; }
        public IIssuedTicketRepository IssuedTicketRepository { get; }
    }
}
