using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Features.EventCancellationRequests.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using UnauthorizedAccessException = BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException;

namespace Catalog.Application.Features.EventCancellationRequests.Commands.RequestEventCancellation
{
    public class RequestEventCancellationCommandHandler : IRequestHandler<RequestEventCancellationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;

        public RequestEventCancellationCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(RequestEventCancellationCommand command, CancellationToken cancellationToken)
        {
            return await RequestEventCancellationAsync(command.EventId, command.Request, command.OrganizerId, cancellationToken);
        }

        private async Task<bool> RequestEventCancellationAsync(
            Guid eventId,
            RequestEventCancellationRequest request,
            Guid organizerId,
            CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ValidatorException(nameof(request.Reason), "Vui lòng nhập lý do yêu cầu hủy sự kiện.");

            Event eventEntity = await _unitOfWork.EventRepository.GetOneAsync<Event>(
                filter: e => e.Id == eventId && !e.IsDeleted,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            if (eventEntity.OrganizerId != organizerId)
                throw new UnauthorizedAccessException("Bạn không có quyền yêu cầu hủy sự kiện này.");

            if (eventEntity.Status != EventStatus.Published)
                throw new BusinessRuleException("Chỉ có thể yêu cầu hủy sự kiện đang ở trạng thái đã xuất bản.");

            if (eventEntity.EndAt <= DateTime.UtcNow)
                throw new BusinessRuleException("Sự kiện đã kết thúc, không thể yêu cầu hủy.");

            EventCancellationRequest? existingRequest = await _unitOfWork.EventCancellationRequestRepository.GetOneUntrackedAsync<EventCancellationRequest>(
                filter: r => r.EventId == eventId && r.Status == EventCancellationRequestStatus.Pending,
                cancellation: cancellation);

            if (existingRequest != null)
                throw new BusinessRuleException("Đã có yêu cầu hủy đang chờ xử lý cho sự kiện này.");

            EventCancellationRequest cancellationRequest = new EventCancellationRequest(eventId, eventEntity.Title, organizerId, request.Reason);

            await _unitOfWork.EventCancellationRequestRepository.CreateAsync(cancellationRequest, cancellation);

            await _eventPublisher.PublishAsync(new NotificationRequestedEvent
            {
                TargetRole = Roles.Moderator,
                Type = "EventCancellationRequested",
                Title = "Có yêu cầu huỷ sự kiện mới",
                Message = $"Sự kiện \"{eventEntity.Title}\" vừa có yêu cầu huỷ. Lý do: {request.Reason}",
                LinkUrl = "/moderator/events",
                ReferenceId = cancellationRequest.Id
            }, cancellationToken: cancellation);

            return true;
        }
    }
}
