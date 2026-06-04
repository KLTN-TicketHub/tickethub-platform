using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IVenueRepository : IBaseRepository<Venue, DbContext>
    {
        Task<string> GenerateNextVenueCodeAsync(string venueName, CancellationToken cancellation = default);
    }
}
