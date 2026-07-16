using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventRating : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public Guid UserId { get; set; }

        public string ReviewerName { get; set; }

        public int SoundRating { get; set; }

        public int VisualRating { get; set; }

        public int OrganizationRating { get; set; }

        public int FacilityRating { get; set; }

        public int ServiceRating { get; set; }

        public int PerformanceRating { get; set; }

        public double OverallRating { get; private set; }

        public string? Comment { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        public EventRating(
            Guid eventId,
            Guid userId,
            string reviewerName,
            int soundRating,
            int visualRating,
            int organizationRating,
            int facilityRating,
            int serviceRating,
            int performanceRating,
            string? comment)
        {
            EventId = eventId;
            UserId = userId;
            ReviewerName = reviewerName;
            SoundRating = soundRating;
            VisualRating = visualRating;
            OrganizationRating = organizationRating;
            FacilityRating = facilityRating;
            ServiceRating = serviceRating;
            PerformanceRating = performanceRating;
            Comment = comment;
            OverallRating = CalculateOverallRating(soundRating, visualRating, organizationRating, facilityRating, serviceRating, performanceRating);
        }

        private static double CalculateOverallRating(int soundRating, int visualRating, int organizationRating, int facilityRating, int serviceRating, int performanceRating)
        {
            return Math.Round((soundRating + visualRating + organizationRating + facilityRating + serviceRating + performanceRating) / 6.0, 2);
        }
    }
}
