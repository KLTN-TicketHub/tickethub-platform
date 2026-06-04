namespace Catalog.Application.Features.EventCategories.Requests
{
    public class CreateEventCategoryRequest
    {
        public string CategoryName { get; set; } = default!;
        public string? Description { get; set; }
    }
}
