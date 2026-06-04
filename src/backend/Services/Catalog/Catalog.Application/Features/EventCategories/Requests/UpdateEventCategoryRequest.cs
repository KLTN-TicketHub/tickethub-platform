namespace Catalog.Application.Features.EventCategories.Requests
{
    public class UpdateEventCategoryRequest
    {
        public string CategoryName { get; set; } = default!;
        public string? Description { get; set; }
    }
}
