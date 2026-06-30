using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class WalletTransaction : BaseEntity, IAggregateRoot
    {
        public Guid WalletId { get; set; }

        public Guid? OrderId { get; set; }

        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }

        public string Description { get; set; } = default!;

        public WalletTransaction() { }
        public WalletTransaction(Guid walletId, Guid? orderId, decimal amount, WalletTransactionType type, string description)
        {
            Id = Guid.NewGuid();
            WalletId = walletId;
            OrderId = orderId;
            Amount = amount;
            Type = type;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
    public enum WalletTransactionType
    {
        Revenue = 1,
        Fee = 2,
        Payout = 3
    }
}
