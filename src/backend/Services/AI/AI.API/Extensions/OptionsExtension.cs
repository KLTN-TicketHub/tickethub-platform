using AI.Common.Options;
using BuildingBlocks.Contracts.Options;

namespace AI.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitConfig>(configuration.GetSection("AppSettings:RateLimit"));
            services.Configure<AiOptions>(configuration.GetSection("OpenRouterAI"));

            return services;
        }
    }
}
