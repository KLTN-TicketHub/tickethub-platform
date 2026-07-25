using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog.Domain.Entities
{
    public class Event : SoftDeleteEntity, IAggregateRoot
    {
        public SeatMap? SeatMap { get; set; }
        public Guid? SeatMapId { get; set; }

        public Guid OrganizerId { get; set; }
        public OrganizerSnapshot? Organizer { get; set; }

        public Guid CategoryId { get; set; }
        public EventCategory? Category { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        public string CurrencyCode { get; set; }

        public string CoverImageUrl { get; set; }

        public EventStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        public EventLocation Location { get; set; }

        private readonly List<ShowTime> _showTimes = new List<ShowTime>();
        public IReadOnlyCollection<ShowTime> ShowTimes => _showTimes.AsReadOnly();

        private readonly List<EventRating> _eventRatings = new List<EventRating>();
        public IReadOnlyCollection<EventRating> EventRatings => _eventRatings.AsReadOnly();

        public EventApproval? Approval { get; private set; }

        public EventCancellation? Cancellation { get; private set; }

        public Event(
            Guid organizerId,
            Guid categoryId,
            string title,
            string description,
            DateTime startAt,
            DateTime endAt,
            DateTime saleOpenAt,
            DateTime saleCloseAt,
            string coverImageUrl,
            string currencyCode = "VND",
            Guid? seatMapId = null)
        {
            OrganizerId = organizerId;
            CategoryId = categoryId;
            Title = title;
            Description = description;
            StartAt = startAt;
            EndAt = endAt;
            SaleOpenAt = saleOpenAt;
            SaleCloseAt = saleCloseAt;
            CoverImageUrl = coverImageUrl;
            CurrencyCode = currencyCode;
            SeatMapId = seatMapId;
            Status = EventStatus.PendingApproval;
            Slug = GenerateSlug(title);
        }

        public void SetEventLocation(string venueName, string addressLine, string ward, string district, string provinceCity, string country)
        {
            Location = new EventLocation(venueName, addressLine, ward, district, provinceCity, country);
        }

        public void AddShowTime(ShowTime showTime)
        {
            if (showTime == null) return;
            showTime.EventId = Id;
            _showTimes.Add(showTime);

            StartAt = _showTimes.Min(s => s.StartAt);
            EndAt = _showTimes.Max(s => s.EndAt);
        }

        public void Review(bool isApproved, Guid reviewerUserId, string? reviewerName, string? reason)
        {
            Status = isApproved ? EventStatus.Published : EventStatus.Rejected;
            if (Approval == null)
            {
                Approval = new EventApproval
                {
                    ReviewerUserId = reviewerUserId,
                    ReviewerName = reviewerName,
                    ReviewedAt = DateTime.UtcNow,
                    Reason = reason,
                    EventId = Id
                };
            }
            else
            {
                Approval.ReviewerUserId = reviewerUserId;
                Approval.ReviewerName = reviewerName;
                Approval.ReviewedAt = DateTime.UtcNow;
                Approval.Reason = reason;
            }
        }

        public void Cancel(Guid cancelledByUserId, string? cancelledByName, string? reason)
        {
            Status = EventStatus.Cancelled;
            Cancellation = new EventCancellation
            {
                EventId = Id,
                CancelledByUserId = cancelledByUserId,
                CancelledByName = cancelledByName,
                Reason = reason,
                CancelledAt = DateTime.UtcNow
            };
        }

        public static string GenerateSlug(string title, int maxLen = 200)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            string normalized = title.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            string replaced = Regex.Replace(withoutDiacritics, @"[^A-Za-z0-9\s]+", string.Empty);
            string slug = Regex.Replace(replaced.Trim(), @"\s+", "-").ToLowerInvariant();

            if (slug.Length > maxLen) slug = slug.Substring(0, maxLen).TrimEnd('-');

            return slug;
        }
    }
}
