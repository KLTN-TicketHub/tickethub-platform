using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class WalletTransaction : BaseEntity, IAggregateRoot
    {
        public Wallet Wallet { get; set; }
        public Guid WalletId { get; set; }

        public Guid? OrderId { get; set; }

        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }

        public WalletTransactionStatus Status { get; set; }

        public DateTime ReleaseAt { get; set; }

        public string Description { get; set; } = default!;

        public WalletTransaction(Guid walletId, Guid? orderId, decimal amount, WalletTransactionType type, string description, DateTime releaseAt)
        {
            WalletId = walletId;
            OrderId = orderId;
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
