using RabbitMQ.Client;

namespace Catalog.API.Extensions
{
    public static class HealthCheckExtension
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("PrimaryDbConnection")!, name: "sqlserver")
                .AddRedis(configuration["Redis:ConnectionString"]!, name: "redis")
                .AddRabbitMQ(sp =>
                {
                    ConnectionFactory factory = new ConnectionFactory
                    {
                        HostName = configuration["RabbitMq:Host"],
                        Port = int.Parse(configuration["RabbitMq:Port"]!),
                        UserName = configuration["RabbitMq:Username"],
                        Password = configuration["RabbitMq:Password"],
                        VirtualHost = configuration["RabbitMq:VirtualHost"]
                    };

                    return factory.CreateConnectionAsync();
                }, name: "rabbitmq");

            return services;
        }
    }
}
