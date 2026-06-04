using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Commands.CreateEventCategory
{
    public record CreateEventCategoryCommand(CreateEventCategoryRequest Request) : IRequest<EventCategoryDto>;
}
