using Identity.Common.Options;

namespace Identity.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.Configure<AdminAccount>(configuration.GetSection("AdminAccount"));

            services.Configure<GoogleAuthSettings>(configuration.GetSection("GoogleAuthSettings"));

            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

            services.Configure<AppUrls>(configuration.GetSection("AppUrls"));

            return services;
        }
    }
}
