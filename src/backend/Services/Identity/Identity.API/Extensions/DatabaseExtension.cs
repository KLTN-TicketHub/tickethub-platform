using Identity.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddCustomDb(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("TicketHub.Identity.Db"));
            });

            return services;
        }
    }
}
