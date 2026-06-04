using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.DeleteEventCategory
{
    public record DeleteEventCategoryCommand(Guid Id) : IRequest<Unit>;
}
