namespace Catalog.Application.Features.EventRatings.Requests
{
    public class CreateEventRatingRequest
    {
        public int SoundRating { get; set; }
        public int VisualRating { get; set; }
        public int OrganizationRating { get; set; }
        public int FacilityRating { get; set; }
        public int ServiceRating { get; set; }
        public int PerformanceRating { get; set; }
        public string? Comment { get; set; }
    }
}
