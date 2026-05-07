using BuildingBlocks.Application.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Infrastructure.Services
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        readonly IPublishEndpoint _publishEndpoint;
        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
            => _publishEndpoint.Publish(@event, cancellationToken);
    }
}
