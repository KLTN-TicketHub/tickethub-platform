using Finance.Common.Dtos.Payouts;

namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IPayoutService
    {
        Task<List<EventPendingPayoutDto>> GetEventsPendingPayoutAsync(CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, EventPayoutResultDto? Data)> ReleaseEventFundsAsync(
            Guid eventId,
            decimal appliedRate,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellationToken = default);
    }
}
