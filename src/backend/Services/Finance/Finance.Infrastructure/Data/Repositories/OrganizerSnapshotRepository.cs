using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class OrganizerSnapshotRepository : BaseRepository<OrganizerSnapshot, FinanceDbContext>, IOrganizerSnapshotRepository
    {
        public OrganizerSnapshotRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
