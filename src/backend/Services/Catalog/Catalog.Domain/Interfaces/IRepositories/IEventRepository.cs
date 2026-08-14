using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IEventRepository : IBaseRepository<Event, DbContext>
    {
        Task<Dictionary<EventStatus, int>> GetCountByStatusAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellation = default);

        Task<List<(Guid CategoryId, string CategoryName, int EventCount)>> GetCountByCategoryAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellation = default);
    }
}