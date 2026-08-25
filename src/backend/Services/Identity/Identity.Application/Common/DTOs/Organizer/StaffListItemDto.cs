namespace Identity.Application.Common.DTOs.Organizer
{
    public class StaffListItemDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public bool IsLocked { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
