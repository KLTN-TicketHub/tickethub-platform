using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.UpdateEventCategory
{
    public record UpdateEventCategoryCommand(Guid Id, UpdateEventCategoryRequest Request) : IRequest<EventCategoryDto>;
}
