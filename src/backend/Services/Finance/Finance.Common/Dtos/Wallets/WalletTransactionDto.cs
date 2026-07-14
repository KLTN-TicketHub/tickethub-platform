namespace Finance.Common.Dtos.Wallets
{
    public class WalletTransactionDto
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Type { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? CreatedAt { get; set; }
    }
}
