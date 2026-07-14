namespace Finance.Common.Dtos.Wallets
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public Guid OrganizerId { get; set; }
        public decimal Balance { get; set; }
    }
}
