using BuildingBlocks.Contracts.Models.Pagination;
using Finance.Common.Dtos.Payouts;

namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IPayoutService
    {
        Task RequestPayoutAsync(
            Guid eventId,
            Guid organizerId,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<ProposedPayoutDto>> GetProposedPayoutsAsync(
            Guid organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<EventPayoutResultDto> AcceptPayoutAsync(
            Guid payoutId,
            Guid organizerId,
            CancellationToken cancellationToken = default);

        Task RejectPayoutAsync(
            Guid payoutId,
            Guid organizerId,
            string? reason,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<PayoutRequestDto>> GetPayoutRequestsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<EventPayoutResultDto> ProposePayoutAsync(
            Guid payoutRequestId,
            decimal appliedRate,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellationToken = default);

        Task<EventPayoutStatusDto> GetEventPayoutStatusAsync(
            Guid eventId,
            Guid organizerId,
            CancellationToken cancellationToken = default);
    }
}
