namespace Catalog.Application.Common.DTOs.EventCategories
{
    public class EventCategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryCode { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public decimal RecommendedCommissionRate { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? CreatedAt { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
