using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.API.Extensions
{
    public static class HangfireExtension
    {
        public static IServiceCollection AddCustomHangfire(this IServiceCollection services, string connection)
        {
            SqlServerStorage storage = new SqlServerStorage(connection);

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(storage));

            services.AddHangfireServer();

            JobStorage.Current = storage;

            return services;
        }
    }
}
