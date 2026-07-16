using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Queries.GetEventRatingsForOrganizer
{
    public class GetEventRatingsForOrganizerQueryHandler : IRequestHandler<GetEventRatingsForOrganizerQuery, PaginatedResult<EventRatingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetEventRatingsForOrganizerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResult<EventRatingDto>> Handle(GetEventRatingsForOrganizerQuery query, CancellationToken cancellationToken)
        {
            return await GetEventRatingsForOrganizerAsync(query.EventId, query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<EventRatingDto>> GetEventRatingsForOrganizerAsync(Guid eventId, GetEventRatingsRequest request, CancellationToken cancellationToken)
        {
            _ = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                filter: e => e.Id == eventId && e.OrganizerId == _currentUserService.UserId && !e.IsDeleted,
                cancellation: cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            (IEnumerable<EventRatingDto> ratings, int totalCount) =
                await _unitOfWork.EventRatingRepository.GetPagedAsync(
                    filter: r => r.EventId == eventId,
                    orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                    selector: r => new EventRatingDto
                    {
                        Id = r.Id,
                        EventId = r.EventId,
                        UserId = r.UserId,
                        ReviewerName = r.ReviewerName,
                        SoundRating = r.SoundRating,
                        VisualRating = r.VisualRating,
                        OrganizationRating = r.OrganizationRating,
                        FacilityRating = r.FacilityRating,
                        ServiceRating = r.ServiceRating,
                        PerformanceRating = r.PerformanceRating,
                        OverallRating = r.OverallRating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    },
                    pageNumber: request.PageNumber,
                    pageSize: request.PageSize,
                    cancellationToken: cancellationToken);

            return new PaginatedResult<EventRatingDto>(
                ratings,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
