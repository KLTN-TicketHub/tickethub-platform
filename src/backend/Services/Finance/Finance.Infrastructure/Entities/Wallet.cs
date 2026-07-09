using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class Wallet : BaseEntity, IAggregateRoot
    {
        public Guid OrganizerId { get; set; }

        public decimal Balance { get; private set; }

        public byte[] RowVersion { get; private set; } = default!;

        private readonly List<WalletTransaction> _transactions = new List<WalletTransaction>();
        public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

        public Wallet(Guid organizerId)
        {
            OrganizerId = organizerId;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void Credit(decimal amount)
        {
            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool Debit(decimal amount)
        {
            if (Balance < amount) return false;
            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}
