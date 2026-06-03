using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventCategory : SoftDeleteEntity, IAggregateRoot
    {
        public string CategoryCode { get; set; }

        public string CategoryName { get; set; }

        public string Slug { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        private readonly List<Event> _events = new List<Event>();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
    }
}
