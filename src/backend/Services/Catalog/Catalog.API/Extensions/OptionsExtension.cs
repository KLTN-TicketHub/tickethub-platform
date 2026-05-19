using BuildingBlocks.Contracts.Options;
using Catalog.Common.Options;

namespace Catalog.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitConfig>(configuration.GetSection("AppSettings:RateLimit"));
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

            return services;
        }
    }
}
