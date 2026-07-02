using Inventory.Common.Options;
using Inventory.Infrastructure.Consumers;
using Inventory.Infrastructure.Data.Contexts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Inventory.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<InventoryDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.AddConsumer<EventPublishedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    RabbitMqOptions rabbitMqOptions = context
                        .GetRequiredService<IOptions<RabbitMqOptions>>()
                        .Value;

                    cfg.Host(
                        rabbitMqOptions.Host,
                        rabbitMqOptions.Port,
                        rabbitMqOptions.VirtualHost,
                        h =>
                        {
                            h.Username(rabbitMqOptions.Username);
                            h.Password(rabbitMqOptions.Password);
                        });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
