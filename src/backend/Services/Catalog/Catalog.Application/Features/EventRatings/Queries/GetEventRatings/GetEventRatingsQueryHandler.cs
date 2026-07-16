using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventRatings.Queries.GetEventRatings
{
    public class GetEventRatingsQueryHandler : IRequestHandler<GetEventRatingsQuery, PaginatedResult<EventRatingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventRatingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<EventRatingDto>> Handle(GetEventRatingsQuery query, CancellationToken cancellationToken)
        {
            return await GetEventRatingsAsync(query.EventId, query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<EventRatingDto>> GetEventRatingsAsync(Guid eventId, GetEventRatingsRequest request, CancellationToken cancellationToken)
        {
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
