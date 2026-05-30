using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Data.Contexts;

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
            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();

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
