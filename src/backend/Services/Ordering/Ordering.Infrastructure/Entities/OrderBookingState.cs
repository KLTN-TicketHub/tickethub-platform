using MassTransit;

namespace Ordering.Infrastructure.Entities
{
    public class OrderBookingState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public Guid UserId { get; set; }

        public Guid ShowtimeId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? PaymentLink { get; set; }
        public Guid? TimeoutTokenId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int Version { get; set; }
    }
}
