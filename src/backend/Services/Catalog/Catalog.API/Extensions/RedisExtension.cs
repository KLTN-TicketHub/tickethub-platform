namespace Catalog.API.Extensions
{
    public static class RedisExtension
    {
        public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"];
                options.InstanceName = configuration["Redis:InstanceName"];
            });

            return services;
        }
    }
}
