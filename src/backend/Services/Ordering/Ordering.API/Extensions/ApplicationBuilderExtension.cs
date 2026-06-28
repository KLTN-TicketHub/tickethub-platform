using Ordering.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            OrderingDbContext db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

            IEnumerable<string> appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            bool isFirstTime = !appliedMigrations.Any();

            await db.Database.MigrateAsync();

            return app;
        }
    }
}
