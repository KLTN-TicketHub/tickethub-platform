using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Email;
using BuildingBlocks.Contracts.Events.Event;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.Commands.ReviewEvent
{
    public class ReviewEventCommandHandler : IRequestHandler<ReviewEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFileService _fileService;

        public ReviewEventCommandHandler(IUnitOfWork unitOfWork, IEventPublisher eventPublisher, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _eventPublisher = eventPublisher;
            _fileService = fileService;
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
            Event eventEntity = await _unitOfWork.EventRepository.GetOneAsync<Event>(
                filter: e => e.Id == eventId && !e.IsDeleted,
                include: q => q.Include(e => e.Organizer!)
                               .Include(e => e.ShowTimes)
                               .ThenInclude(s => s.TicketTypes)
                               .ThenInclude(t => t.Zone),
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            if (eventEntity.Status != EventStatus.PendingApproval)
                throw new BusinessRuleException("Chỉ có thể duyệt sự kiện đang ở trạng thái chờ duyệt.");

            if (!request.IsApproved && string.IsNullOrWhiteSpace(request.Reason))
                throw new ValidatorException(nameof(request.Reason), "Vui lòng nhập lý do từ chối sự kiện.");

            eventEntity.Review(request.IsApproved, reviewerUserId, reviewerName, request.Reason);

            await _eventPublisher.PublishAsync(new EventReviewedEvent
            {
                EventId = eventEntity.Id,
                EventTitle = eventEntity.Title,
                OrganizerEmail = eventEntity.Organizer?.Email ?? string.Empty,
                OrganizerName = eventEntity.Organizer?.OrganizerName ?? string.Empty,
                IsApproved = request.IsApproved,
                Reason = request.Reason
            }, cancellationToken: cancellation);

            if (request.IsApproved)
            {
                var eventPublished = new EventPublishedEvent
                {
                    EventId = eventEntity.Id,
                    EventTitle = eventEntity.Title,
                    EventImage = _fileService.GetAbsoluteUrl(eventEntity.CoverImageUrl),
                    OrganizerId = eventEntity.OrganizerId,
                    OrganizerName = eventEntity.Organizer?.OrganizerName ?? string.Empty,
                    SaleOpenAt = eventEntity.SaleOpenAt,
                    SaleCloseAt = eventEntity.SaleCloseAt,
                    Showtimes = eventEntity.ShowTimes.Select(s => new ShowtimePublishedDto
                    {
                        ShowTimeId = s.Id,
                        StartAt = s.StartAt,
                        EndAt = s.EndAt,
                        TicketTypes = s.TicketTypes.Select(t => new TicketTypePublishedDto
                        {
                            TicketTypeId = t.Id,
                            TicketTypeName = t.TicketTypeName,
                            Capacity = t.PublishedQuota,
                            Price = t.Price,
                            IsReservingSeat = t.Zone?.IsReservingSeat ?? false
                        }).ToList()
                    }).ToList()
                };

                await _eventPublisher.PublishAsync(eventPublished, cancellationToken: cancellation);
            }

            await _unitOfWork.EventRepository.SaveChangeAsync(cancellation);

            return true;
        }
    }
}
