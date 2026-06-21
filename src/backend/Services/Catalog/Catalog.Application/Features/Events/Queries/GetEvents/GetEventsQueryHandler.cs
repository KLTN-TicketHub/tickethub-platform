using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Linq.Expressions;

namespace Catalog.Application.Features.Events.Queries.GetEvents
{
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, PaginatedResult<EventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetEventsQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<PaginatedResult<EventListItemDto>> Handle(GetEventsQuery query, CancellationToken cancellationToken)
        {
            return await GetEventsAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<EventListItemDto>> GetEventsAsync(GetEventsRequest request, CancellationToken cancellationToken)
        {
            var search = request.Search?.Trim();

            Expression<Func<Event, bool>> filter = e =>
                !e.IsDeleted &&
                e.Status == EventStatus.Published &&
                (string.IsNullOrEmpty(search) || e.Title.Contains(search)) &&
                (request.CategoryId == Guid.Empty || e.CategoryId == request.CategoryId) &&
                (!request.FromDate.HasValue || e.StartAt >= request.FromDate.Value) &&
                (!request.ToDate.HasValue || e.StartAt <= request.ToDate.Value) &&
                (string.IsNullOrEmpty(request.ProvinceCity) || e.Location.ProvinceCity == request.ProvinceCity);

            (IEnumerable<EventListItemDto> events, int totalCount) =
                await _unitOfWork.EventRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(e => e.CreatedAt),
                    selector: e => new EventListItemDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Slug = e.Slug,
                        StartAt = e.StartAt,
                        EndAt = e.EndAt,
                        CoverImageUrl = _fileService.GetAbsoluteUrl(e.CoverImageUrl),
                        CategoryName = e.Category!.CategoryName,
                        MinPrice = e.ShowTimes.SelectMany(st => st.TicketTypes).Min(tt => tt.Price),
                        ProvinceCity = e.Location.ProvinceCity
                    },
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    cancellationToken: cancellationToken);

            return new PaginatedResult<EventListItemDto>(
                events,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
