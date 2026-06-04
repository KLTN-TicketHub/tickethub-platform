using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IEventCategoryRepository : IBaseRepository<EventCategory, DbContext>
    {
        Task<string> GenerateNextCategoryCodeAsync(string categoryName, CancellationToken cancellation = default);
    }
}