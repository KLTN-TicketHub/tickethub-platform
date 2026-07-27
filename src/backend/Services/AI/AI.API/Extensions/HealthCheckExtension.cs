namespace AI.API.Extensions
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddRedis(configuration["Redis:ConnectionString"]!, name: "redis");

            return services;
        }
    }
}
