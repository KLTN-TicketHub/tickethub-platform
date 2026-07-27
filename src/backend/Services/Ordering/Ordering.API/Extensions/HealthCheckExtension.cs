namespace Ordering.API.Extensions
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            string rabbitMqUri = $"amqp://{configuration["RabbitMq:Username"]}:{configuration["RabbitMq:Password"]}@{configuration["RabbitMq:Host"]}:{configuration["RabbitMq:Port"]}/";

            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("PrimaryDbConnection")!, name: "sqlserver")
                .AddRedis(configuration["Redis:ConnectionString"]!, name: "redis")
                .AddRabbitMQ(rabbitMqUri, name: "rabbitmq");

            return services;
        }
    }
}
