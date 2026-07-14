using BuildingBlocks.Contracts.Models.Pagination;
using Finance.Common.Dtos.Payouts;

namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IPayoutService
    {
        Task<(bool IsSuccess, string Message)> RequestPayoutAsync(
            Guid eventId,
            Guid organizerId,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<ProposedPayoutDto>> GetProposedPayoutsAsync(
            Guid organizerId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, EventPayoutResultDto? Data)> AcceptPayoutAsync(
            Guid payoutId,
            Guid organizerId,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<PayoutRequestDto>> GetPayoutRequestsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, EventPayoutResultDto? Data)> ProposePayoutAsync(
            Guid payoutRequestId,
            decimal appliedRate,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellationToken = default);
    }
}
