namespace BuildingBlocks.Domain.DDD
{
    public class BaseEntity
    {
        public virtual Guid Id { get; set; }

        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Guid? CreateBy { get; set; }

        public virtual DateTime? UpdateAt { get; set; }

        public virtual Guid? UpdateBy { get; set; }

        public virtual void SetCreated(Guid? updatedBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreateBy = updatedBy;
        }

        public virtual void SetUpdated(Guid? updatedBy)
        {
            UpdateAt = DateTime.UtcNow;
            UpdateBy = updatedBy;
        }
    }
}
