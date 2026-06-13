using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class OrganizerSnapshot : SoftDeleteEntity, IAggregateRoot
    {
        public string FullName { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatAt { get; set; }


    }
}
