using MassTransit;
using Microsoft.Extensions.Options;
using Ordering.Common.Options;
using Ordering.Infrastructure.Consumers;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Sagas;

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

                x.AddDelayedMessageScheduler();

                x.AddConsumer<ConfirmOrderConsumer>();
                x.AddConsumer<CancelOrderConsumer>();
                x.AddConsumer<EventPublishedConsumer>();
                x.AddConsumer<EventCancelledConsumer>();
                x.AddConsumer<FinalizeOrderRefundConsumer>();

                x.AddSagaStateMachine<OrderBookingStateMachine, OrderBookingState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        r.ExistingDbContext<OrderingDbContext>();
                    });

                x.AddSagaStateMachine<OrderRefundStateMachine, OrderRefundState>()
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

                    cfg.UseDelayedMessageScheduler();

                    cfg.ReceiveEndpoint("ordering-confirm-order", e =>
                    {
                        e.ConfigureConsumer<ConfirmOrderConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("ordering-cancel-order", e =>
                    {
                        e.ConfigureConsumer<CancelOrderConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("ordering-event-published", e =>
                    {
                        e.ConfigureConsumer<EventPublishedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("ordering-event-cancelled", e =>
                    {
                        e.ConfigureConsumer<EventCancelledConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("ordering-finalize-refund", e =>
                    {
                        e.ConfigureConsumer<FinalizeOrderRefundConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
