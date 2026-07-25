using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventCancellationRequests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventCancellationRequests.Queries.GetEventCancellationRequests
{
    public class GetEventCancellationRequestsQueryHandler : IRequestHandler<GetEventCancellationRequestsQuery, PaginatedResult<EventCancellationRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventCancellationRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<EventCancellationRequestDto>> Handle(GetEventCancellationRequestsQuery query, CancellationToken cancellationToken)
        {
            return await GetEventCancellationRequestsAsync(query.PageNumber, query.PageSize, cancellationToken);
        }

        private async Task<PaginatedResult<EventCancellationRequestDto>> GetEventCancellationRequestsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellation = default)
        {
            (IEnumerable<EventCancellationRequestDto> items, int totalCount) = await _unitOfWork.EventCancellationRequestRepository.GetPagedAsync(
                filter: r => r.Status == EventCancellationRequestStatus.Pending,
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                selector: r => new EventCancellationRequestDto
                {
                    Id = r.Id,
                    EventId = r.EventId,
                    EventTitle = r.EventTitle,
                    OrganizerId = r.OrganizerId,
                    OrganizerName = string.Empty,
                    Reason = r.Reason,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    ReviewerName = r.ReviewerName,
                    ReviewedAt = r.ReviewedAt,
                    RejectionReason = r.RejectionReason
                },
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellation);

            List<EventCancellationRequestDto> data = items.ToList();

            List<Guid> organizerIds = data.Select(d => d.OrganizerId).Distinct().ToList();
            IEnumerable<OrganizerSnapshot> organizers = await _unitOfWork.OrganizerSnapshotRepository.GetAllAsync<OrganizerSnapshot>(
                filter: o => organizerIds.Contains(o.Id),
                cancellation: cancellation);

            Dictionary<Guid, string> organizerNames = organizers.ToDictionary(o => o.Id, o => o.OrganizerName);

            foreach (EventCancellationRequestDto item in data)
            {
                if (organizerNames.TryGetValue(item.OrganizerId, out string? name))
                    item.OrganizerName = name;
            }

            return new PaginatedResult<EventCancellationRequestDto>(data, totalCount, pageNumber, pageSize);
        }
    }
}
