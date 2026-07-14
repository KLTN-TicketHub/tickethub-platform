using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class WalletTransaction : BaseEntity, IAggregateRoot
    {
        public Wallet Wallet { get; set; }
        public Guid WalletId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid EventId { get; set; }

        public string EventTitle { get; set; } = default!;

        public Guid CategoryId { get; set; }

        public Guid? EventPayoutId { get; private set; }

        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }

        public WalletTransactionStatus Status { get; set; }

        public DateTime ReleaseAt { get; set; }

        public string Description { get; set; } = default!;

        public WalletTransaction(
            Guid walletId,
            Guid? orderId,
            Guid eventId,
            string eventTitle,
            Guid categoryId,
            decimal amount,
            WalletTransactionType type,
            string description,
            DateTime releaseAt)
        {
            WalletId = walletId;
            OrderId = orderId;
            EventId = eventId;
            EventTitle = eventTitle;
            CategoryId = categoryId;
            Amount = amount;
            Type = type;
            Description = description;
            Status = WalletTransactionStatus.Pending;
            ReleaseAt = releaseAt;
        }

        public void MarkAsSuccess()
        {
            Status = WalletTransactionStatus.Success;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AttachToPayout(Guid eventPayoutId)
        {
            EventPayoutId = eventPayoutId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DetachFromPayout()
        {
            EventPayoutId = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignPayout(Guid eventPayoutId)
        {
            EventPayoutId = eventPayoutId;
            MarkAsSuccess();
        }
    }
    public enum WalletTransactionType
    {
        Revenue = 1,
        Fee = 2,
        Payout = 3
    }
    public enum WalletTransactionStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }
}
