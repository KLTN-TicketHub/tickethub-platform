namespace Identity.Application.Common.DTOs.Auth
{
    public class ProfileDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? ImageUrl { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public DateTime? CreateAt { get; set; }
    }
}