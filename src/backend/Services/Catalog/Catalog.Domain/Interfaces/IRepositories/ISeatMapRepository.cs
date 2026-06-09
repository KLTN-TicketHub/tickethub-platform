using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface ISeatMapRepository : IBaseRepository<SeatMap, DbContext>
    {
        Task<string> GenerateNextSeatMapCodeAsync(Guid venueId, string seatMapName, CancellationToken cancellation = default);
    }
}