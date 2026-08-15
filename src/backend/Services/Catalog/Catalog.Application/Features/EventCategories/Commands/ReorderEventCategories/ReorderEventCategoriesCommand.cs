using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.ReorderEventCategories
{
    public class CategoryOrderDto
    {
        public Guid CategoryId { get; set; }
        public int DisplayOrder { get; set; }
    }

    public record ReorderEventCategoriesCommand(List<CategoryOrderDto> Categories) : IRequest<bool>;
}
