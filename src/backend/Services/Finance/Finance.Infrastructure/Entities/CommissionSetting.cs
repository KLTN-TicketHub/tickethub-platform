using BuildingBlocks.Domain.DDD;

namespace Finance.Infrastructure.Entities
{
    public class CommissionSetting : BaseEntity, IAggregateRoot
    {
        public Guid CategoryId { get; private set; }

        public string CategoryName { get; private set; } = default!;

        public decimal Rate { get; private set; }

        public CommissionSetting(Guid categoryId, string categoryName, decimal rate)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            Rate = rate;
        }

        public void SyncFromCatalog(string categoryName, decimal rate)
        {
            CategoryName = categoryName;
            Rate = rate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
