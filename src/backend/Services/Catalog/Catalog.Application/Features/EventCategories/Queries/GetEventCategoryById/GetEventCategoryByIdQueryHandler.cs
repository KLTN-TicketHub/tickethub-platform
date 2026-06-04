using AutoMapper;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCategories.Queries.GetEventCategoryById
{
    public class GetEventCategoryByIdQueryHandler : IRequestHandler<GetEventCategoryByIdQuery, EventCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventCategoryDto> Handle(GetEventCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetEventCategoryByIdAsync(query.Id, cancellationToken);
        }

        private async Task<EventCategoryDto> GetEventCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork
                .EventCategoryRepository
                .GetOneUntrackedAsync(
                filter: ec => ec.Id == id && !ec.IsDeleted,
                selector: ec => new EventCategoryDto
                {
                    Id = id,
                    CategoryCode = ec.CategoryCode,
                    CategoryName = ec.CategoryName,
                    Slug = ec.Slug,
                    Description = ec.Description,
                    Status = ec.Status.ToString(),
                    CreatedAt = ec.CreatedAt,
                    RowVersion = ec.RowVersion
                },
                cancellation: cancellationToken) ?? throw new NotFoundException("Không tìm thấy danh mục sự kiện");
        }
    }
}
