using Notification.Worker;
using Notification.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SendEmailCodeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");

        cfg.Host(
            rabbitMqSettings["Host"] ?? "localhost",
            ushort.Parse(rabbitMqSettings["Port"] ?? "5672"),
            rabbitMqSettings["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbitMqSettings["UserName"] ?? "guest");
                h.Password(rabbitMqSettings["Password"] ?? "guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
