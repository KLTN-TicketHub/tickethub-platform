using Inventory.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            InventoryDbContext db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            IEnumerable<string> appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            bool isFirstTime = !appliedMigrations.Any();

            await db.Database.MigrateAsync();

            return app;
        }
    }
}
