namespace Identity.Application.Common.DTOs.Admin
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? ImageUrl { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public bool IsLocked { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
