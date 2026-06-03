using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class SeatMapRepository : BaseRepository<SeatMap, DbContext>, ISeatMapRepository
    {
        public SeatMapRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}