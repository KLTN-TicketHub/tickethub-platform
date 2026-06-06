using BuildingBlocks.Domain.Outbox;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public class OutboxProcessor<TContext> : BackgroundService where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OutboxProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
                    IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
                    List<OutboxMessage> messages = await context.Set<OutboxMessage>()
                        .Where(x => x.ProcessedOn == null)
                        .OrderBy(x => x.OccurredOn)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    if (messages.Any())
                    {
                        foreach (var msg in messages)
                        {
                            try
                            {
                                Type? type = Type.GetType(msg.Type);
                                if (type == null) throw new Exception($"Không tìm thấy type: {msg.Type}");
                                object? evt = JsonSerializer.Deserialize(msg.Payload, type);
                                await publishEndpoint.Publish(evt!, stoppingToken);
                                msg.ProcessedOn = DateTime.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                msg.Error = ex.Message;
                                msg.RetryCount++;
                            }
                        }
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
