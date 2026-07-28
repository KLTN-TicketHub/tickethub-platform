namespace Ordering.API.Extensions
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            string rabbitMqUsername = Uri.EscapeDataString(configuration["RabbitMq:Username"]!);
            string rabbitMqPassword = Uri.EscapeDataString(configuration["RabbitMq:Password"]!);
            string rabbitMqUri = $"amqp://{rabbitMqUsername}:{rabbitMqPassword}@{configuration["RabbitMq:Host"]}:{configuration["RabbitMq:Port"]}/";

            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("PrimaryDbConnection")!, name: "sqlserver")
                .AddRedis(configuration["Redis:ConnectionString"]!, name: "redis")
                .AddRabbitMQ(rabbitMqUri, name: "rabbitmq");

            return services;
        }
    }
}
