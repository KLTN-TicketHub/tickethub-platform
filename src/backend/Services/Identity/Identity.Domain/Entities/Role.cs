using BuildingBlocks.Domain.DDD;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class Role : IdentityRole<Guid>, IAggregateRoot
    {
    }
}
