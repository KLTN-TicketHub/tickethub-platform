namespace Catalog.Application.Common.DTOs.Profiles
{
    public class OrganizerProfileDto
    {
        public Guid Id { get; set; }

        public string OrganizerName { get; set; }

        public string? ImageUrl { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
