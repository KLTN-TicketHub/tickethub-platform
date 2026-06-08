using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class Event : SoftDeleteEntity, IAggregateRoot
    {
        public Venue? Venue { get; set; }
        public Guid? VenueId { get; set; }

        public SeatMap? SeatMap { get; set; }
        public Guid? SeatMapId { get; set; }

        public Guid OrganizerId { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        //Đơn vị tiền tệ (VD: 'USD', 'VND')
        public string CurrencyCode { get; set; }

        public string CoverImageUrl { get; set; }

        //Trạng thái (VD: 'Published', 'PendingApproval')
        public EventStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        public EventLocation? Location { get; set; }

        private readonly List<EventCategory> _categories = new List<EventCategory>();
        public IReadOnlyCollection<EventCategory> Categories => _categories.AsReadOnly();

        private readonly List<TicketType> _ticketTypes = new List<TicketType>();
        public IReadOnlyCollection<TicketType> TicketTypes => _ticketTypes.AsReadOnly();

        public void SetEventLocation(string venueName, string addressLine, string ward, string district, string provinceCity, string country)
        {
            Location = new EventLocation(venueName, addressLine, ward, district, provinceCity, country);
        }
    }
}
