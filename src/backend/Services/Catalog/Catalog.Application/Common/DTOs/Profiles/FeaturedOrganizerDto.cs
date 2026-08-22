namespace Catalog.Application.Common.DTOs.Profiles
{
    public class FeaturedOrganizerDto
    {
        public Guid Id { get; set; }

        public string OrganizerName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int PublishedEventCount { get; set; }
    }
}
