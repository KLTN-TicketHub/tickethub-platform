using Finance.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Finance.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            FinanceDbContext db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

            IEnumerable<string> appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
            bool isFirstTime = !appliedMigrations.Any();

            await db.Database.MigrateAsync();

            return app;
        }
    }
}
