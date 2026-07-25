using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IEventClickStatRepository : IBaseRepository<EventClickStat, DbContext>
    {
        Task<List<(DateOnly StatDate, EventClickType ClickType, long Total)>> GetTrendByEventAsync(
            Guid eventId, DateOnly from, DateOnly to, CancellationToken cancellation = default);

        Task<List<(DateOnly StatDate, EventClickType ClickType, long Total)>> GetTrendByOrganizerAsync(
            Guid organizerId, DateOnly from, DateOnly to, CancellationToken cancellation = default);

        Task<List<(Guid EventId, string EventTitle, long ViewCount, long PurchaseIntentCount)>> GetTopEventsByOrganizerAsync(
            Guid organizerId, DateOnly from, DateOnly to, int top, CancellationToken cancellation = default);

        Task<List<(Guid CategoryId, string CategoryName, long ViewCount, long PurchaseIntentCount, int ActiveEventCount)>> GetCategoryTrendAsync(
            DateOnly from, DateOnly to, CancellationToken cancellation = default);
    }
}
