using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Contracts.Events.Event;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Features.EventCancellationRequests.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.EventCancellationRequests.Commands.ReviewEventCancellationRequest
{
    public class ReviewEventCancellationRequestCommandHandler : IRequestHandler<ReviewEventCancellationRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;

        public ReviewEventCancellationRequestCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> Handle(ReviewEventCancellationRequestCommand command, CancellationToken cancellationToken)
        {
            return await ReviewEventCancellationRequestAsync(
                requestId: command.RequestId,
                request: command.Request,
                reviewerUserId: command.ReviewerUserId,
                reviewerName: command.ReviewerName,
                cancellation: cancellationToken);
        }

        private async Task<bool> ReviewEventCancellationRequestAsync(
            Guid requestId,
            ReviewEventCancellationRequestRequest request,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellation = default)
        {
            EventCancellationRequest cancellationRequest = await _unitOfWork.EventCancellationRequestRepository.GetOneAsync<EventCancellationRequest>(
                filter: r => r.Id == requestId,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy yêu cầu hủy sự kiện với ID {requestId}.");

            if (cancellationRequest.Status != EventCancellationRequestStatus.Pending)
                throw new BusinessRuleException("Yêu cầu hủy sự kiện này đã được xử lý.");

            if (!request.IsApproved && string.IsNullOrWhiteSpace(request.Reason))
                throw new ValidatorException(nameof(request.Reason), "Vui lòng nhập lý do từ chối yêu cầu hủy sự kiện.");

            Event eventEntity = await _unitOfWork.EventRepository.GetOneAsync<Event>(
                filter: e => e.Id == cancellationRequest.EventId && !e.IsDeleted,
                include: q => q.Include(e => e.Organizer!),
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {cancellationRequest.EventId}.");

            if (request.IsApproved)
            {
                if (eventEntity.Status != EventStatus.Published)
                    throw new BusinessRuleException("Chỉ có thể hủy sự kiện đang ở trạng thái đã xuất bản.");

                if (eventEntity.EndAt <= DateTime.UtcNow)
                    throw new BusinessRuleException("Sự kiện đã kết thúc, không thể hủy.");

                eventEntity.Cancel(reviewerUserId, reviewerName, cancellationRequest.Reason);
                cancellationRequest.Approve(reviewerUserId, reviewerName);

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
                    Reason = cancellationRequest.Reason
                }, cancellationToken: cancellation);

                await _eventPublisher.PublishAsync(new NotificationRequestedEvent
                {
                    RecipientUserId = eventEntity.OrganizerId,
                    Type = "EventCancellationApproved",
                    Title = "Yêu cầu huỷ sự kiện được chấp thuận",
                    Message = $"Yêu cầu huỷ sự kiện \"{eventEntity.Title}\" đã được chấp thuận. Hệ thống sẽ tiến hành hoàn tiền cho các đơn hàng liên quan.",
                    LinkUrl = $"/organizer/events/{eventEntity.Id}",
                    ReferenceId = eventEntity.Id
                }, cancellationToken: cancellation);
            }
            else
            {
                cancellationRequest.Reject(reviewerUserId, reviewerName, request.Reason!);

                await _eventPublisher.PublishAsync(new EventCancellationRequestReviewedEvent
                {
                    EventId = eventEntity.Id,
                    EventTitle = eventEntity.Title,
                    OrganizerEmail = eventEntity.Organizer?.Email ?? string.Empty,
                    OrganizerName = eventEntity.Organizer?.OrganizerName ?? string.Empty,
                    IsApproved = false,
                    Reason = request.Reason
                }, cancellationToken: cancellation);

                await _eventPublisher.PublishAsync(new NotificationRequestedEvent
                {
                    RecipientUserId = eventEntity.OrganizerId,
                    Type = "EventCancellationRejected",
                    Title = "Yêu cầu huỷ sự kiện bị từ chối",
                    Message = $"Yêu cầu huỷ sự kiện \"{eventEntity.Title}\" đã bị từ chối. Lý do: {request.Reason}",
                    LinkUrl = $"/organizer/events/{eventEntity.Id}",
                    ReferenceId = eventEntity.Id
                }, cancellationToken: cancellation);
            }

            await _unitOfWork.EventCancellationRequestRepository.SaveChangeAsync(cancellation);

            return true;
        }
    }
}
