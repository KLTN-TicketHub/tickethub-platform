namespace Catalog.Application.Common.DTOs.EventRatings
{
    public class EventRatingDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int SoundRating { get; set; }
        public int VisualRating { get; set; }
        public int OrganizationRating { get; set; }
        public int FacilityRating { get; set; }
        public int ServiceRating { get; set; }
        public int PerformanceRating { get; set; }
        public double OverallRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
