namespace BuildingBlocks.Contracts.Events.EventCategory
{
    public class EventCategoryCommissionRateChangedEvent
    {
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public decimal RecommendedCommissionRate { get; init; }
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
