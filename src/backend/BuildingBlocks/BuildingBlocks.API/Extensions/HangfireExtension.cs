using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions
{
    public static class HangfireExtension
    {
        public static IServiceCollection AddCustomHangfire(this IServiceCollection services, string connection)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connection));

            services.AddHangfireServer();

            return services;
        }
    }
}
