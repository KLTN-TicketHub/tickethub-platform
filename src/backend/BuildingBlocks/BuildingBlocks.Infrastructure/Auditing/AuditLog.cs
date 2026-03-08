namespace BuildingBlocks.Infrastructure.Auditing
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? CorrelationId { get; set; }
        public string? Source { get; set; }
        public string? RequestPath { get; set; }
    }
}
