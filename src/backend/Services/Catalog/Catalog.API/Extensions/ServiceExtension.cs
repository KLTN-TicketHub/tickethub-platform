using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Services;

namespace Catalog.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
