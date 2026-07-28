using Finance.Common.Options;
using Finance.Infrastructure.Data.Contexts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Finance.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<Finance.Infrastructure.Consumers.OrderPaidConsumer>();
                x.AddConsumer<Finance.Infrastructure.Consumers.EventCategoryCommissionRateChangedConsumer>();
                x.AddConsumer<Finance.Infrastructure.Consumers.OrganizerActivatedConsumer>();
                x.AddConsumer<Finance.Infrastructure.Consumers.OrderRefundedConsumer>();

                x.AddEntityFrameworkOutbox<FinanceDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
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

                    cfg.ReceiveEndpoint("finance-order-paid", e =>
                    {
                        e.ConfigureConsumer<Finance.Infrastructure.Consumers.OrderPaidConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("finance-event-category-commission-rate-changed", e =>
                    {
                        e.ConfigureConsumer<Finance.Infrastructure.Consumers.EventCategoryCommissionRateChangedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("finance-order-refunded", e =>
                    {
                        e.ConfigureConsumer<Finance.Infrastructure.Consumers.OrderRefundedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("finance-organizer-activated", e =>
                    {
                        e.ConfigureConsumer<Finance.Infrastructure.Consumers.OrganizerActivatedConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
