using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            return services;
        }

        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ICacheService, RedisCacheService>();
            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<AuditInterceptor>();
            return services;
        }
    }
}
