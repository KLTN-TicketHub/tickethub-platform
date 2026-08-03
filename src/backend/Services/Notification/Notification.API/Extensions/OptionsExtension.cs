using BuildingBlocks.Contracts.Options;
using Notification.Common.Options;

namespace Notification.API.Extensions
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
