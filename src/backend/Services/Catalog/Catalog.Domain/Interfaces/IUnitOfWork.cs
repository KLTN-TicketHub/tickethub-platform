using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Interfaces.IRepositories;

namespace Catalog.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IVenueRepository VenueRepository { get; }
        IEventRepository EventRepository { get; }
        IEventCategoryRepository EventCategoryRepository { get; }
        ISeatRepository SeatRepository { get; }
        ISeatMapRepository SeatMapRepository { get; }
        ITicketTypeRepository TicketTypeRepository { get; }
        IZoneRepository ZoneRepository { get; }
        IOrganizerSnapshotRepository OrganizerSnapshotRepository { get; }
        IEventRatingRepository EventRatingRepository { get; }
        IEventCheckInRepository EventCheckInRepository { get; }
    }
}
