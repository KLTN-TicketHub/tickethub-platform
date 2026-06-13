namespace BuildingBlocks.Domain.DDD
{
    public class BaseEntity
    {
        public virtual Guid Id { get; set; }

        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Guid? CreatedBy { get; set; }

        public virtual DateTime? UpdatedAt { get; set; }

        public virtual Guid? UpdatedBy { get; set; }

        public virtual void SetCreated(Guid? createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public virtual void SetUpdated(Guid? updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
