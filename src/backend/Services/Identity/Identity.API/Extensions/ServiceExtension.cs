using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Infrastructure.Services;
using Identity.Application.Common.Interfaces.IDataSeedingServices;
using Identity.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Identity.Domain.Interfaces;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;
using Identity.Infrastructure.Data.Contexts;
using Identity.Infrastructure.Data.DataSeedingServices;
using Identity.Infrastructure.Data.Repositories.Identity_AuthRepositories;
using Identity.Infrastructure.ExternalServices.TokenServices;

namespace Identity.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            RegisterSeedData(services);
            return services;
        }

        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }

        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            return services;
        }

        public static IServiceCollection RegisterSeedData(IServiceCollection services)
        {
            services.AddScoped<IDataSeedingService, DataSeedingService>();
            return services;
        }
    }
}
