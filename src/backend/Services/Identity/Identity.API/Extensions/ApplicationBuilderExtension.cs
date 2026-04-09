using Identity.Application.Common.Interfaces.IDataSeedingServices;
using Identity.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            IdentityDbContext db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

            IEnumerable<string> appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            bool isFirstTime = !appliedMigrations.Any();

            await db.Database.MigrateAsync();

            if (isFirstTime)
            {
                IDataSeedingService seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
                await seedingService.SeedDataAsync();
            }

            return app;
        }
    }
}
