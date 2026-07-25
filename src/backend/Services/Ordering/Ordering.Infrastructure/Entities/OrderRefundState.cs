using MassTransit;

namespace Ordering.Infrastructure.Entities
{
    public class OrderRefundState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public Guid EventId { get; set; }

        public decimal RefundableAmount { get; set; }

        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int Version { get; set; }
    }
}
