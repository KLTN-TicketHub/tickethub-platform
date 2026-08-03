using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Common.DTOs.Profiles;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Events.Commands.CreateEvent
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IEventPublisher _eventPublisher;

        public CreateEventCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileService fileService,
            IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _eventPublisher = eventPublisher;
        }

        public async Task<EventDto> Handle(CreateEventCommand command, CancellationToken cancellation = default)
        {
            return await CreateEventAsync(command.OrganizerId, command.Request, cancellation);
        }

        private async Task<EventDto> CreateEventAsync(Guid organizerId, CreateEventRequest request, CancellationToken cancellation = default)
        {
            OrganizerSnapshot organizer = await _unitOfWork.OrganizerSnapshotRepository.GetOneUntrackedAsync<OrganizerSnapshot>(
                filter: o => o.Id == organizerId && !o.IsDeleted,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy organizer với ID {organizerId}.");

            EventCategory category = await _unitOfWork.EventCategoryRepository.GetOneUntrackedAsync<EventCategory>(
                filter: c => c.Id == request.CategoryId && !c.IsDeleted,
                cancellation: cancellation)
                    ?? throw new NotFoundException($"Không tìm thấy danh mục với ID {request.CategoryId}.");

            if (!_fileService.FileExists(request.CoverImageUrl))
                throw new NotFoundException($"Không tìm thấy ảnh bìa tại đường dẫn '{request.CoverImageUrl}'. Vui lòng tải ảnh lên trước.");

            if (request.SeatMapId.HasValue)
            {
                if (!await _unitOfWork.SeatMapRepository.IsExistsAsync(nameof(SeatMap.Id), request.SeatMapId.Value))
                    throw new NotFoundException($"Không tìm thấy sơ đồ chỗ ngồi với ID {request.SeatMapId.Value}.");

                foreach (var showTimeRequest in request.ShowTimes)
                {
                    foreach (var ticketTypeRequest in showTimeRequest.TicketTypes)
                    {
                        if (!ticketTypeRequest.ZoneId.HasValue)
                            throw new ValidatorException(nameof(ticketTypeRequest.ZoneId),
                                $"Loại vé '{ticketTypeRequest.TicketTypeName}' phải có mã khu vực khi sử dụng sơ đồ chỗ ngồi.");

                        Zone zone = await _unitOfWork.ZoneRepository.GetOneUntrackedAsync<Zone>(
                            filter: z => z.Id == ticketTypeRequest.ZoneId && !z.IsDeleted && z.IsSalable,
                            cancellation: cancellation) ?? throw new NotFoundException($"Không tìm thấy khu vực với ID {ticketTypeRequest.ZoneId.Value}.");

                        if (ticketTypeRequest.PublishedQuota.HasValue && ticketTypeRequest.PublishedQuota.Value > zone.Capacity)
                            throw new ValidatorException(nameof(ticketTypeRequest.PublishedQuota),
                                $"Số lượng vé được xuất bản cho loại vé '{ticketTypeRequest.TicketTypeName}' không được vượt quá sức chứa của khu vực ({zone.Capacity} vé).");
                    }
                }
            }

            Event newEvent = _mapper.Map<Event>(request);
            newEvent.OrganizerId = organizerId;

            if (request.ShowTimes != null)
            {
                foreach (var showTimeRequest in request.ShowTimes)
                {
                    ShowTime showTime = new ShowTime(showTimeRequest.StartAt, showTimeRequest.EndAt);

                    if (showTimeRequest.TicketTypes is { Count: > 0 })
                    {
                        var orderedTicketTypes = showTimeRequest.TicketTypes.OrderBy(tt => tt.DisplayOrder).ToList();

                        int displayOrder = 0;
                        foreach (var ticketTypeRequest in orderedTicketTypes)
                        {
                            TicketType ticketType = _mapper.Map<TicketType>(ticketTypeRequest);
                            ticketType.DisplayOrder = displayOrder++;
                            showTime.AddTicketType(ticketType);
                        }
                    }

                    newEvent.AddShowTime(showTime);
                }
            }

            if (request.SeatMapId == null && request.Location != null)
            {
                newEvent.SetEventLocation(
                    request.Location.VenueName,
                    request.Location.AddressLine,
                    request.Location.Ward,
                    request.Location.District,
                    request.Location.ProvinceCity,
                    request.Location.Country);
            }
            else
            {
                SeatMap? seatMap = await _unitOfWork.SeatMapRepository.GetOneUntrackedAsync<SeatMap>(
                    filter: sm => sm.Id == request.SeatMapId && !sm.IsDeleted,
                    include: sm => sm.Include(sm => sm.Venue),
                    cancellation: cancellation);

                newEvent.SetEventLocation(
                    seatMap!.Venue!.VenueName,
                    seatMap.Venue.AddressLine,
                    seatMap.Venue.Ward,
                    seatMap.Venue.District,
                    seatMap.Venue.ProvinceCity,
                    seatMap.Venue.Country);
            }

            Event created = await _unitOfWork.EventRepository.CreateAsync(newEvent, cancellation);

            await PublishPendingReviewNotificationAsync(created, organizer, cancellation);

            EventDto result = _mapper.Map<EventDto>(created);
            result.CategoryName = category.CategoryName;
            result.OrganizerProfile = _mapper.Map<OrganizerProfileDto>(organizer);
            result.CoverImageUrl = _fileService.GetFileUrl(newEvent.CoverImageUrl);

            return result;
        }

        private async Task PublishPendingReviewNotificationAsync(
            Event created,
            OrganizerSnapshot organizer,
            CancellationToken cancellation)
        {
            await _eventPublisher.PublishAsync(new NotificationRequestedEvent
            {
                TargetRole = Roles.Moderator,
                Type = "EventPendingReview",
                Title = "Có sự kiện mới chờ duyệt",
                Message = $"Nhà tổ chức {organizer.OrganizerName} vừa gửi sự kiện \"{created.Title}\" để chờ duyệt.",
                LinkUrl = $"/moderator/events/{created.Id}",
                ReferenceId = created.Id
            }, cancellationToken: cancellation);
        }
    }
}
