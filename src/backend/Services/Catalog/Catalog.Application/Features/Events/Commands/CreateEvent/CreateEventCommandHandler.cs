using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Common.DTOs.Profiles;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Commands.CreateEvent
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CreateEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
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

                foreach (var ticketTypeRequest in request.TicketTypes)
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

            Event newEvent = _mapper.Map<Event>(request);
            newEvent.OrganizerId = organizerId;

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

            if (request.TicketTypes is { Count: > 0 })
            {
                request.TicketTypes = request.TicketTypes.OrderBy(tt => tt.DisplayOrder).ToList();

                int displayOrder = 0;
                foreach (var ticketTypeRequest in request.TicketTypes)
                {
                    TicketType ticketType = _mapper.Map<TicketType>(ticketTypeRequest);
                    ticketType.DisplayOrder = displayOrder++;
                    newEvent.AddTicketType(ticketType);
                }
            }

            Event created = await _unitOfWork.EventRepository.CreateAsync(newEvent, cancellation);

            EventDto result = _mapper.Map<EventDto>(created);
            result.CategoryName = category.CategoryName;
            result.OrganizerProfile = _mapper.Map<OrganizerProfileDto>(organizer);
            result.CoverImageUrl = _fileService.GetFileUrl(newEvent.CoverImageUrl);

            return result;
        }
    }
}
