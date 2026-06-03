using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Interfaces.IRepositories;

namespace Catalog.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IVenueRepository VenueRepository { get; }
    }
}
