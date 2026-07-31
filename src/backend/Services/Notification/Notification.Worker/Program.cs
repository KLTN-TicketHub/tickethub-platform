using BuildingBlocks.Logging;
using Notification.Infrastructure.Factories.Email;
using Notification.Worker;
using Notification.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCustomLogging(builder.Configuration, "Notification");

builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddScoped<MailServiceFactory>();

builder.Services.AddMassTransitWithRabbitMq();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
