using BuildingBlocks.Infrastructure.Data;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;
using Identity.Infrastructure.Data.Contexts;

namespace Identity.Infrastructure.Data.Repositories.Identity_AuthRepositories
{
    public class UserRepository : BaseRepository<User, IdentityDbContext>, IUserRepository
    {
        public UserRepository(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
