using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Data.Repositories;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.API.Extensions
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
            services.AddScoped<IEventPublisher, MassTransitEventPublisher<InventoryDbContext>>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IFileService, FileService>();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IShowtimeTicketInventoryRepository, ShowtimeTicketInventoryRepository>();
            services.AddScoped<IShowtimeSeatRepository, ShowtimeSeatRepository>();
            return services;
        }
    }
}
