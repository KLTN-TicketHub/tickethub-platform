namespace Identity.Application.Common.DTOs.Auth
{
    public class OrganizerDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string OrganizerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
