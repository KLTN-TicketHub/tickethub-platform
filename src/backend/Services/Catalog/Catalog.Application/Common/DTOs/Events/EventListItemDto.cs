namespace Catalog.Application.Common.DTOs.Events
{
    public class EventListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ProvinceCity { get; set; } = string.Empty;
    }
}
