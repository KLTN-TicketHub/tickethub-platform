using Catalog.Application.Common.DTOs.EventCategories;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Queries.GetEventCategoryById
{
    public record GetEventCategoryByIdQuery(Guid Id) : IRequest<EventCategoryDto>;
}
