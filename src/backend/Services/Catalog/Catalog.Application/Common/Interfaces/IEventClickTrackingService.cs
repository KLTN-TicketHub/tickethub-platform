using Catalog.Domain.Enums;

namespace Catalog.Application.Common.Interfaces
{
    public interface IEventClickTrackingService
    {
        Task RecordClickAsync(Guid eventId, EventClickType clickType, Guid? userId, CancellationToken cancellationToken = default);

        Task<List<(Guid EventId, EventClickType ClickType, long Delta)>> GetAndResetCountersAsync(CancellationToken cancellationToken = default);

        Task<List<(Guid EventId, Guid UserId, EventClickType ClickType, DateTime ClickedAt)>> DrainUserClicksAsync(CancellationToken cancellationToken = default);
    }
}
