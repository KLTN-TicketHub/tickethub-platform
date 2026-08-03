using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Notification.API.Services;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Data.Repositories;
using Notification.Infrastructure.Interfaces;
using Notification.Infrastructure.Interfaces.IRepositories;
using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.API.Extensions
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
            services.AddScoped<INotificationPushService, NotificationPushService>();
            services.AddScoped<INotificationService, Notification.Infrastructure.Services.NotificationService>();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUserNotificationReadRepository, UserNotificationReadRepository>();
            services.AddScoped<IScheduledNotificationRepository, ScheduledNotificationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
