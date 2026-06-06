using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class MassTransitEventPublisher<TContext> : IEventPublisher where TContext : DbContext
    {
        readonly TContext _context;

        public MassTransitEventPublisher(TContext context)
        {
            _context = context;
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : class
        {
            OutboxMessage message = new()
            {
                Id = Guid.NewGuid(),
                Type = typeof(TEvent).AssemblyQualifiedName!,
                Payload = JsonSerializer.Serialize(@event),
                OccurredOn = DateTime.UtcNow
            };

            _context.Set<OutboxMessage>().Add(message);
        }
    }
}
