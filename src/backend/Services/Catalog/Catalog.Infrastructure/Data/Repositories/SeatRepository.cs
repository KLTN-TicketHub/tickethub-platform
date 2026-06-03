using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class SeatRepository : BaseRepository<Seat, DbContext>, ISeatRepository
    {
        public SeatRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}