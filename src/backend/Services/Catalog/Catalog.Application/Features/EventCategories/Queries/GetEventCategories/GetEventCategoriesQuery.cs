using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Queries.GetEventCategories
{
    public record GetEventCategoriesQuery(GetCategoriesRequest Request) : IRequest<PaginatedResult<EventCategoryDto>>;
}
