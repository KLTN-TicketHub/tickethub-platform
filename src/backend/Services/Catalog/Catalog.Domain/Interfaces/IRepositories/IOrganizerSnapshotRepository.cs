using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IOrganizerSnapshotRepository : IBaseRepository<OrganizerSnapshot, DbContext>
    {
        Task<List<(Guid Id, string OrganizerName, string? ImageUrl, int PublishedEventCount)>> GetFeaturedOrganizersAsync(
            int count,
            CancellationToken cancellation = default);
    }
}
