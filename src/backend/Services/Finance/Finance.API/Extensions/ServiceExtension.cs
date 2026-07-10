using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Services;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Data.Repositories;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.API.Extensions
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
            services.AddScoped<Finance.Infrastructure.Services.IReleaseFundsJobService, Finance.Infrastructure.Services.ReleaseFundsJobService>();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
