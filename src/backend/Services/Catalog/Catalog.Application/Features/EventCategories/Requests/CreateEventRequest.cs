namespace Catalog.Application.Features.EventCategories.Requests
{
    public class CreateEventRequest
    {
        public string CategoryName { get; set; } = default!;

        public string? Description { get; set; }
    }
}
