using BuildingBlocks.Contracts.Events.Organizer;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Consumers
{
    public class OrganizerAvatarUpdatedConsumer : IConsumer<OrganizerAvatarUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganizerAvatarUpdatedConsumer> _logger;

        public OrganizerAvatarUpdatedConsumer(
            IUnitOfWork unitOfWork,
            ILogger<OrganizerAvatarUpdatedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrganizerAvatarUpdatedEvent> context)
        {
            OrganizerAvatarUpdatedEvent message = context.Message;

            OrganizerSnapshot? existing = await _unitOfWork.OrganizerSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

            if (existing == null)
            {
                _logger.LogWarning("Received OrganizerAvatarUpdatedEvent for unknown OrganizerId: {OrganizerId}", message.Id);
                return;
            }

            existing.ImageUrl = message.ImageUrl;

            await _unitOfWork.OrganizerSnapshotRepository.UpdateAsync(existing, context.CancellationToken);

            _logger.LogInformation("Successfully updated avatar for OrganizerId: {OrganizerId}", message.Id);
        }
    }
}
