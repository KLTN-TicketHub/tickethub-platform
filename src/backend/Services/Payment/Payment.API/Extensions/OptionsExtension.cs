using BuildingBlocks.Contracts.Options;
using Payment.Common.Options;

namespace Payment.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitConfig>(configuration.GetSection("AppSettings:RateLimit"));
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
            services.Configure<PaymentOptions>(configuration.GetSection("Payment"));

            return services;
        }
    }
}
