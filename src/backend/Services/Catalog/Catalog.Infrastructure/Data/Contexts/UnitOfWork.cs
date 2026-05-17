using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
    {
        public UnitOfWork(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
