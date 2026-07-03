using MassTransit;
using Microsoft.Extensions.Options;
using Ordering.Common.Options;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Consumers;
using Ordering.Infrastructure.Sagas;
using Ordering.Infrastructure.Entities;

namespace Ordering.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<OrderingDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.AddConsumer<ConfirmOrderConsumer>();
                x.AddConsumer<CancelOrderConsumer>();

                x.AddSagaStateMachine<OrderBookingStateMachine, OrderBookingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.ExistingDbContext<OrderingDbContext>();
                    });

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
