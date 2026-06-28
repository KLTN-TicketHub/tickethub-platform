using BuildingBlocks.Infrastructure.Auditing;
using Finance.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Finance.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddCustomDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<FinanceDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PrimaryDbConnection"));

                options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
            });

            return services;
        }
    }
}
