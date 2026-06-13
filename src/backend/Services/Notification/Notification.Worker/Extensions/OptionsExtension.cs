using Notification.Common.Options;

namespace Notification.Worker.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.Configure<AppUrls>(configuration.GetSection("AppUrls"));

            return services;
        }
    }
}
