namespace YarpApiGateway.Extensions
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();

            return services;
        }
    }
}
