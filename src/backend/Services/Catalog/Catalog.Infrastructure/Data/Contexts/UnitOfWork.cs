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
            ISeatRepository seatRepository,
            ISeatMapRepository seatMapRepository,
            ITicketTypeRepository ticketTypeRepository,
            IZoneRepository zoneRepository,
            IOrganizerSnapshotRepository organizerSnapshotRepository,
            IEventRatingRepository eventRatingRepository,
            IEventCheckInRepository eventCheckInRepository) : base(dbContext)
        {
            VenueRepository = venueRepository;
            EventRepository = eventRepository;
            EventCategoryRepository = eventCategoryRepository;
            SeatRepository = seatRepository;
            SeatMapRepository = seatMapRepository;
            TicketTypeRepository = ticketTypeRepository;
            ZoneRepository = zoneRepository;
            OrganizerSnapshotRepository = organizerSnapshotRepository;
            EventRatingRepository = eventRatingRepository;
            EventCheckInRepository = eventCheckInRepository;
        }

        public IVenueRepository VenueRepository { get; set; }
        public IEventRepository EventRepository { get; set; }
        public IEventCategoryRepository EventCategoryRepository { get; set; }
        public ISeatRepository SeatRepository { get; set; }
        public ISeatMapRepository SeatMapRepository { get; set; }
        public ITicketTypeRepository TicketTypeRepository { get; set; }
        public IZoneRepository ZoneRepository { get; set; }
        public IOrganizerSnapshotRepository OrganizerSnapshotRepository { get; set; }
        public IEventRatingRepository EventRatingRepository { get; set; }
        public IEventCheckInRepository EventCheckInRepository { get; set; }
    }
}
