using BuildingBlocks.Application.Interfaces;
using MassTransit;

namespace BuildingBlocks.Infrastructure.Services
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
        }
    }
}
