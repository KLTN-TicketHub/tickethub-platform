using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Payment.Infrastructure.Data.Contexts;
using Payment.Infrastructure.Data.Repositories;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IRepositories;
using Payment.Infrastructure.Interfaces.IServices;
using Payment.Infrastructure.Services;

namespace Payment.API.Extensions
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
            services.AddScoped<IVnpayService, VnpayService>();
            services.AddHttpClient();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
