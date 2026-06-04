using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class EventCategory : SoftDeleteEntity, IAggregateRoot
    {
        public string CategoryCode { get; set; }

        public string CategoryName { get; set; }

        public string Slug { get; set; }

        public string? Description { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        private readonly List<Event> _events = new List<Event>();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

        public EventCategory(string categoryName, string? description)
        {
            CategoryName = categoryName;
            Description = description;
        }
    }
}
