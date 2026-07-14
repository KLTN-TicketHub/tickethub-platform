using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Domain.Exceptions;

namespace Finance.Infrastructure.Entities
{
    public class EventPayout : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; private set; }

        public string EventTitle { get; private set; } = default!;

        public Guid CategoryId { get; private set; }

        public Guid OrganizerId { get; private set; }

        public Guid WalletId { get; private set; }

        public decimal GrossAmount { get; private set; }

        public decimal RecommendedRate { get; private set; }

        public decimal AppliedRate { get; private set; }

        public decimal FeeAmount { get; private set; }

        public decimal NetAmount { get; private set; }

        public EventPayoutStatus Status { get; private set; }

        public Guid ReviewedByUserId { get; private set; }

        public string? ReviewedByName { get; private set; }

        public DateTime ReviewedAt { get; private set; }

        public DateTime? AcceptedAt { get; private set; }

        public string? RejectionReason { get; private set; }

        public DateTime? RejectedAt { get; private set; }

        public EventPayout(
            Guid eventId,
            string eventTitle,
            Guid categoryId,
            Guid organizerId,
            Guid walletId,
            decimal grossAmount,
            decimal recommendedRate,
            decimal appliedRate,
            Guid reviewedByUserId,
            string? reviewedByName)
        {
            EventId = eventId;
            EventTitle = eventTitle;
            CategoryId = categoryId;
            OrganizerId = organizerId;
            WalletId = walletId;
            GrossAmount = grossAmount;
            RecommendedRate = recommendedRate;
            AppliedRate = appliedRate;
            FeeAmount = Math.Round(grossAmount * appliedRate / 100m, 2);
            NetAmount = grossAmount - FeeAmount;
            Status = EventPayoutStatus.Proposed;
            ReviewedByUserId = reviewedByUserId;
            ReviewedByName = reviewedByName;
            ReviewedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            if (Status != EventPayoutStatus.Proposed)
                throw new BusinessRuleException("Chỉ có thể chấp nhận đề xuất giải ngân đang ở trạng thái chờ xác nhận.");

            Status = EventPayoutStatus.Accepted;
            AcceptedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(string? reason)
        {
            if (Status != EventPayoutStatus.Proposed)
                throw new BusinessRuleException("Chỉ có thể từ chối đề xuất giải ngân đang ở trạng thái chờ xác nhận.");

            Status = EventPayoutStatus.Rejected;
            RejectionReason = reason;
            RejectedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum EventPayoutStatus
    {
        Proposed = 1,
        Accepted = 2,
        Rejected = 3
    }
}
