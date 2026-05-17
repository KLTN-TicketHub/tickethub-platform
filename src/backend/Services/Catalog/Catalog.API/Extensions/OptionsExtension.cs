using BuildingBlocks.Contracts.Options;

namespace Catalog.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitConfig>(configuration.GetSection("AppSettings:RateLimit"));

            return services;
        }
    }
}
