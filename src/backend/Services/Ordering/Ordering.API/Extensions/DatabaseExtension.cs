using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data.Contexts;

namespace Ordering.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddCustomDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<OrderingDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PrimaryDbConnection"));

                options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
            });

            return services;
        }
    }
}
