using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Catalog.Application.Features.Events.Commands.ReviewEvent
{
    public class ReviewEventCommandHandler : IRequestHandler<ReviewEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReviewEventCommand command, CancellationToken cancellationToken)
        {
            return await ReviewEventAsync(
                eventId: command.EventId,
                request: command.Request,
                reviewerUserId: command.ReviewerUserId,
                reviewerName: command.ReviewerName,
                cancellation: cancellationToken);
        }

        private async Task<bool> ReviewEventAsync(
            Guid eventId,
            ReviewEventRequest request,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellation = default)
        {
            Event eventEntity = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                filter: e => e.Id == eventId && !e.IsDeleted,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            if (eventEntity.Status != EventStatus.PendingApproval)
                throw new BusinessRuleException("Chỉ có thể duyệt sự kiện đang ở trạng thái chờ duyệt.");

            if (!request.IsApproved && string.IsNullOrWhiteSpace(request.Reason))
                throw new ValidatorException(nameof(request.Reason), "Vui lòng nhập lý do từ chối sự kiện.");

            eventEntity.Review(request.IsApproved, reviewerUserId, reviewerName, request.Reason);

            await _unitOfWork.EventRepository.UpdateAsync(eventEntity, cancellation);

            return true;
        }
    }
}
