using BuildingBlocks.Contracts.Extensions;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Queries.GetEventCategories
{
    public class GetEventCategoriesQueryHandler : IRequestHandler<GetEventCategoriesQuery, PaginatedResult<EventCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<EventCategoryDto>> Handle(GetEventCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await GetEventCategoriesAsync(query.Request);
        }

        private async Task<PaginatedResult<EventCategoryDto>> GetEventCategoriesAsync(GetCategoriesRequest request)
        {
            (IEnumerable<EventCategoryDto> eventCategories, int totalCount) = await _unitOfWork.EventCategoryRepository.GetPagedAsync(
                filter: e => !e.IsDeleted,
                orderBy: e => e.OrderBy(e => e.CategoryName),
                selector: e => new EventCategoryDto
                {
                    Id = e.Id,
                    CategoryCode = e.CategoryCode,
                    CategoryName = e.CategoryName,
                    Slug = e.Slug,
                    Description = e.Description,
                    Status = e.Status.GetDisplayName(),
                    CreatedAt = e.CreatedAt,
                    RowVersion = e.RowVersion
                },
                pageNumber: request.PageNumber,
                pageSize: request.PageSize
            );

            return new PaginatedResult<EventCategoryDto>(eventCategories, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
