using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IEventCheckInRepository : IBaseRepository<EventCheckIn, DbContext>
    {
    }
}
