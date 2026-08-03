using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.Data.Contexts;

namespace Notification.API.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> UseDatabaseInitialization(this IApplicationBuilder app)
        {
            await using AsyncServiceScope scope = app.ApplicationServices.CreateAsyncScope();

            NotificationDbContext db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

            await db.Database.MigrateAsync();

            return app;
        }
    }
}
