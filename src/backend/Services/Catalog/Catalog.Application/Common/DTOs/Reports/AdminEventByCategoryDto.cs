namespace Catalog.Application.Common.DTOs.Reports
{
    public class AdminEventByCategoryDto
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = default!;

        public int EventCount { get; set; }
    }
}
