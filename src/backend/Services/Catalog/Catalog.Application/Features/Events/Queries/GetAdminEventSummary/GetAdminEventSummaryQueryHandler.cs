using Catalog.Application.Common.DTOs.Reports;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetAdminEventSummary
{
    public class GetAdminEventSummaryQueryHandler : IRequestHandler<GetAdminEventSummaryQuery, AdminEventSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminEventSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminEventSummaryDto> Handle(GetAdminEventSummaryQuery query, CancellationToken cancellation = default)
        {
            return await GetSummaryAsync(query.From, query.To, cancellation);
        }

        private async Task<AdminEventSummaryDto> GetSummaryAsync(DateTime from, DateTime to, CancellationToken cancellation)
        {
            Dictionary<EventStatus, int> countByStatus = await _unitOfWork.EventRepository.GetCountByStatusAsync(from, to, cancellation);

            return new AdminEventSummaryDto
            {
                TotalEvents = countByStatus.Values.Sum(),
                PendingApprovalCount = countByStatus.GetValueOrDefault(EventStatus.PendingApproval),
                PublishedCount = countByStatus.GetValueOrDefault(EventStatus.Published),
                RejectedCount = countByStatus.GetValueOrDefault(EventStatus.Rejected),
                CancelledCount = countByStatus.GetValueOrDefault(EventStatus.Cancelled),
                ArchivedCount = countByStatus.GetValueOrDefault(EventStatus.Archived)
            };
        }
    }
}
