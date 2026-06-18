namespace Catalog.Application.Common.DTOs.Events
{
    public class OrganizerEventListItemDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string CoverImageUrl { get; set; }

        public string Status { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public EventLocationDto Location { get; set; }
    }
}
