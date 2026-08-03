using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Contracts.Events.Event;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.Commands.CancelEvent
{
    public class CancelEventCommandHandler : IRequestHandler<CancelEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;

        public CancelEventCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(CancelEventCommand command, CancellationToken cancellationToken)
        {
            return await CancelEventAsync(command.EventId, command.ModeratorUserId, command.ModeratorName, cancellationToken);
        }

        private async Task<bool> CancelEventAsync(
            Guid eventId,
            Guid moderatorUserId,
            string? moderatorName,
            CancellationToken cancellation = default)
        {
            Event eventEntity = await _unitOfWork.EventRepository.GetOneAsync<Event>(
                filter: e => e.Id == eventId && !e.IsDeleted,
                include: q => q.Include(e => e.Organizer!),
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            if (eventEntity.Status != EventStatus.Published)
                throw new BusinessRuleException("Chỉ có thể hủy sự kiện đang ở trạng thái đã xuất bản.");

            if (eventEntity.EndAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Sự kiện đã kết thúc, không thể hủy.");

            eventEntity.Cancel(moderatorUserId, moderatorName, reason: null);

            await _eventPublisher.PublishAsync(new EventCancelledEvent
            {
                EventId = eventEntity.Id,
                EventTitle = eventEntity.Title,
                OrganizerId = eventEntity.OrganizerId,
                OrganizerName = eventEntity.Organizer?.OrganizerName ?? string.Empty,
                CancelledAt = eventEntity.Cancellation!.CancelledAt
            }, cancellationToken: cancellation);

            await _eventPublisher.PublishAsync(new EventCancelledEmailEvent
            {
                EventId = eventEntity.Id,
                EventTitle = eventEntity.Title,
                OrganizerEmail = eventEntity.Organizer?.Email ?? string.Empty,
                OrganizerName = eventEntity.Organizer?.OrganizerName ?? string.Empty,
                Reason = null
            }, cancellationToken: cancellation);

            await _eventPublisher.PublishAsync(new NotificationRequestedEvent
            {
                RecipientUserId = eventEntity.OrganizerId,
                Type = "EventCancelled",
                Title = "Sự kiện đã bị huỷ",
                Message = $"Sự kiện \"{eventEntity.Title}\" đã bị huỷ bởi quản trị viên. Hệ thống sẽ tiến hành hoàn tiền cho các đơn hàng liên quan.",
                LinkUrl = $"/organizer/events/{eventEntity.Id}",
                ReferenceId = eventEntity.Id
            }, cancellationToken: cancellation);

            await _unitOfWork.EventRepository.SaveChangeAsync(cancellation);

            return true;
        }
    }
}
