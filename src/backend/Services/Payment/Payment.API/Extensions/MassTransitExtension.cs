using MassTransit;
using Microsoft.Extensions.Options;
using Payment.Common.Options;
using Payment.Infrastructure.Consumers;
using Payment.Infrastructure.Data.Contexts;

namespace Payment.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<PaymentDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.AddConsumer<GeneratePaymentLinkConsumer>();
                x.AddConsumer<RefundPaymentConsumer>();

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

                    cfg.ReceiveEndpoint("payment-generate-link", e =>
                    {
                        e.ConfigureConsumer<GeneratePaymentLinkConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("payment-refund-order", e =>
                    {
                        e.ConfigureConsumer<RefundPaymentConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
