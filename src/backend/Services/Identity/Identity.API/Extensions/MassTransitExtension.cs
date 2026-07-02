using Identity.Common.Options;
using Identity.Infrastructure.Data.Contexts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Identity.API.Extensions
{
    public static class MassTransitExtension
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
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

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
