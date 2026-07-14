using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class PayoutRequest : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; private set; }

        public string EventTitle { get; private set; } = default!;

        public Guid CategoryId { get; private set; }

        public Guid OrganizerId { get; private set; }

        public Guid WalletId { get; private set; }

        public PayoutRequestStatus Status { get; private set; }

        public Guid? EventPayoutId { get; private set; }

        public PayoutRequest(Guid eventId, string eventTitle, Guid categoryId, Guid organizerId, Guid walletId)
        {
            EventId = eventId;
            EventTitle = eventTitle;
            CategoryId = categoryId;
            OrganizerId = organizerId;
            WalletId = walletId;
            Status = PayoutRequestStatus.Pending;
        }

        public void MarkAsProcessed(Guid eventPayoutId)
        {
            Status = PayoutRequestStatus.Processed;
            EventPayoutId = eventPayoutId;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum PayoutRequestStatus
    {
        Pending = 1,
        Processed = 2
    }
}
