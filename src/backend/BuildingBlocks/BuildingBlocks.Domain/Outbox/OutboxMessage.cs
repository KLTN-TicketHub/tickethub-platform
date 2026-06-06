using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Domain.Outbox
{
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(500)]
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;

        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public string? Error { get; set; }
        public int RetryCount { get; set; }
    }
}
