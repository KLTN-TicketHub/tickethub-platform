using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Inventory.API.Services;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Data.Repositories;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IRepositories;
using Inventory.Infrastructure.Interfaces.IServices;
using Inventory.Infrastructure.Services;

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
            services.AddScoped<IRedisLockService, RedisLockService>();
            services.AddScoped<ISeatStateService, SeatStateService>();
            services.AddScoped<ISeatHubNotificationService, SeatHubNotificationService>();
            services.AddScoped<ITicketInventoryService, TicketInventoryService>();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IShowtimeTicketInventoryRepository, ShowtimeTicketInventoryRepository>();
            services.AddScoped<IShowtimeSeatRepository, ShowtimeSeatRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
