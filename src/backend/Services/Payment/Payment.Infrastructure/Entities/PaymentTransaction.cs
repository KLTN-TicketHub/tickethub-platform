using BuildingBlocks.Domain.DDD;

namespace Payment.Infrastructure.Entities
{
    public class PaymentTransaction : BaseEntity, IAggregateRoot
    {
        public Guid OrderId { get; set; }

        public string MerchantOrderNo { get; set; } = default!;

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Gateway { get; set; } = default!;

        public PaymentStatus Status { get; set; }

        public string? RawGatewayResponse { get; set; }

        public DateTime? CompletedAt { get; set; }

        public PaymentTransaction(Guid orderId, string merchantOrderNo, decimal amount, string gateway)
        {
            OrderId = orderId;
            MerchantOrderNo = merchantOrderNo;
            Amount = amount;
            Gateway = gateway;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
        public void MarkAsSuccess(string transactionId, string rawResponse)
        {
            Status = PaymentStatus.Success;
            TransactionId = transactionId;
            RawGatewayResponse = rawResponse;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public void MarkAsFailed(string? transactionId, string rawResponse)
        {
            Status = PaymentStatus.Failed;
            TransactionId = transactionId;
            RawGatewayResponse = rawResponse;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4
    }
}
