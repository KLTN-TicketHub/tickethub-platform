using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class OrganizerSnapshot : BaseEntity, IAggregateRoot
    {
        public string OrganizerName { get; set; } = default!;

        public string? ImageUrl { get; set; }

        public string Email { get; set; } = default!;

        public string? PhoneNumber { get; set; }
    }
}