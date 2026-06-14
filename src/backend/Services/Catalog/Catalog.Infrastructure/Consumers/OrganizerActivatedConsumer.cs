using BuildingBlocks.Contracts.Events.Organizer;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Consumers
{
    public class OrganizerActivatedConsumer : IConsumer<OrganizerActivatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganizerActivatedConsumer> _logger;

        public OrganizerActivatedConsumer(
            IUnitOfWork unitOfWork,
            ILogger<OrganizerActivatedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrganizerActivatedEvent> context)
        {
            OrganizerActivatedEvent message = context.Message;

            _logger.LogInformation(
                "Received OrganizerActivatedEvent: Id={OrganizerId}, FullName={FullName}, CorrelationId={CorrelationId}",
                message.Id,
                message.FullName,
                message.CorrelationId);

            OrganizerSnapshot? existing = await _unitOfWork.OrganizerSnapshotRepository.GetByIdAsync(message.Id, context.CancellationToken);

            if (existing == null)
            {
                OrganizerSnapshot snapshot = new OrganizerSnapshot
                {
                    Id = message.Id,
                    OrganizerName = message.FullName,
                    ImageUrl = message.ImageUrl,
                    Email = message.Email,
                    PhoneNumber = message.PhoneNumber,
                };

                await _unitOfWork.OrganizerSnapshotRepository.CreateAsync(snapshot, context.CancellationToken);

                _logger.LogInformation("Successfully created new OrganizerSnapshot for OrganizerId: {OrganizerId}", message.Id);
            }
            else
            {
                existing.OrganizerName = message.FullName;
                existing.ImageUrl = message.ImageUrl;
                existing.Email = message.Email;
                existing.PhoneNumber = message.PhoneNumber;

                await _unitOfWork.OrganizerSnapshotRepository.UpdateAsync(existing, context.CancellationToken);

                _logger.LogInformation("Successfully updated OrganizerSnapshot for OrganizerId: {OrganizerId}", message.Id);
            }
        }
    }
}
