using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Interfaces.IRepositories;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<CatalogDbContext>, IUnitOfWork
    {
        public UnitOfWork(CatalogDbContext dbContext, IVenueRepository venueRepository) : base(dbContext)
        {
            VenueRepository = venueRepository;
        }

        public IVenueRepository VenueRepository { get; set; }
    }
}
