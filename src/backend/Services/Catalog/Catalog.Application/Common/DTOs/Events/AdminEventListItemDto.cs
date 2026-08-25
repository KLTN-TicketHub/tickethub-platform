namespace Catalog.Application.Common.DTOs.Events
{
    public class AdminEventListItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string OrganizerName { get; set; } = string.Empty;

        public decimal MinPrice { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public EventLocationDto Location { get; set; } = default!;
    }
}
