using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Data.Contexts;

namespace Payment.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddCustomDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<PaymentDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PrimaryDbConnection"));

                options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
            });

            return services;
        }
    }
}
