using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class OrganizerSnapshot : SoftDeleteEntity, IAggregateRoot
    {
        public string OrganizerName { get; set; }

        public string? ImageUrl { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        private readonly List<Event> _events = new List<Event>();
        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
    }
}
