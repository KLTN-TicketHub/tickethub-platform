using AI.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            AiDbContext db = scope.ServiceProvider.GetRequiredService<AiDbContext>();

            await db.Database.MigrateAsync();

            return app;
        }
    }
}
