using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.IRepositories;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<CatalogDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            CatalogDbContext dbContext,
            IVenueRepository venueRepository,
            IEventRepository eventRepository,
            IEventCategoryRepository eventCategoryRepository,
            IEventApprovalRepository eventApprovalRepository,
            ISeatRepository seatRepository,
            ISeatMapRepository seatMapRepository,
            ITicketTypeRepository ticketTypeRepository,
            IZoneRepository zoneRepository) : base(dbContext)
        {
            VenueRepository = venueRepository;
            EventRepository = eventRepository;
            EventCategoryRepository = eventCategoryRepository;
            EventApprovalRepository = eventApprovalRepository;
            SeatRepository = seatRepository;
            SeatMapRepository = seatMapRepository;
            TicketTypeRepository = ticketTypeRepository;
            ZoneRepository = zoneRepository;
        }

        public IVenueRepository VenueRepository { get; set; }
        public IEventRepository EventRepository { get; set; }
        public IEventCategoryRepository EventCategoryRepository { get; set; }
        public IEventApprovalRepository EventApprovalRepository { get; set; }
        public ISeatRepository SeatRepository { get; set; }
        public ISeatMapRepository SeatMapRepository { get; set; }
        public ITicketTypeRepository TicketTypeRepository { get; set; }
        public IZoneRepository ZoneRepository { get; set; }
    }
}
