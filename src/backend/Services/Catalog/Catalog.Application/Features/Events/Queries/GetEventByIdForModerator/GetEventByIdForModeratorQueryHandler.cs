using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Extensions;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventByIdForModerator
{
    public class GetEventByIdForModeratorQueryHandler : IRequestHandler<GetEventByIdForModeratorQuery, EventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetEventByIdForModeratorQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<EventDto> Handle(GetEventByIdForModeratorQuery query, CancellationToken cancellationToken = default)
        {
            return await GetEventByIdForModeratorAsync(query.Id, cancellationToken);
        }

        private async Task<EventDto> GetEventByIdForModeratorAsync(Guid id, CancellationToken cancellation = default)
        {
            return await _unitOfWork.EventRepository.GetOneUntrackedAsync(
                filter: e => e.Id == id && !e.IsDeleted,
                selector: e => new EventDto
                {
                    Id = id,
                    SeatMapId = e.SeatMapId,
                    VenueId = e.SeatMap != null ? e.SeatMap.VenueId : null,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category!.CategoryName,
                    Title = e.Title,
                    Slug = e.Slug,
                    Description = e.Description,
                    StartAt = e.StartAt,
                    EndAt = e.EndAt,
                    SaleOpenAt = e.SaleOpenAt,
                    SaleCloseAt = e.SaleCloseAt,
                    CurrencyCode = e.CurrencyCode,
                    CoverImageUrl = _fileService.GetAbsoluteUrl(e.CoverImageUrl),
                    Status = e.Status.GetDisplayName(),
                    CreatedAt = e.CreatedAt,
                    RowVersion = e.RowVersion,
                    Location = new EventLocationDto
                    {
                        VenueName = e.Location.VenueName,
                        AddressLine = e.Location.AddressLine,
                        Ward = e.Location.Ward,
                        District = e.Location.District,
                        ProvinceCity = e.Location.ProvinceCity,
                        Country = e.Location.Country
                    },
                    OrganizerProfile = new Common.DTOs.Profiles.OrganizerProfileDto
                    {
                        Id = e.Organizer!.Id,
                        OrganizerName = e.Organizer.OrganizerName,
                        ImageUrl = e.Organizer.ImageUrl != null ? _fileService.GetAbsoluteUrl(e.Organizer.ImageUrl) : null,
                        Email = e.Organizer.Email,
                        PhoneNumber = e.Organizer.PhoneNumber,
                        CreatedAt = e.Organizer.CreatedAt
                    },
                    Showtimes = e.ShowTimes.Select(st => new ShowtimeDto
                    {
                        Id = st.Id,
                        EventId = st.EventId,
                        StartAt = st.StartAt,
                        EndAt = st.EndAt,
                        Status = st.Status.GetDisplayName(),
                        TicketTypes = st.TicketTypes.Select(tt => new TicketTypeDto
                        {
                            Id = tt.Id,
                            ZoneId = tt.ZoneId,
                            TicketTypeName = tt.TicketTypeName,
                            Description = tt.Description,
                            Price = tt.Price,
                            PublishedQuota = tt.PublishedQuota,
                            MaxQtyQuota = tt.MaxQtyQuota,
                            MinQtyQuota = tt.MinQtyQuota,
                            DisplayOrder = tt.DisplayOrder,
                            Status = tt.Status.GetDisplayName(),
                            RowVersion = tt.RowVersion,
                        }).OrderBy(tt => tt.DisplayOrder).ToList()
                    }).OrderBy(st => st.StartAt).ToList(),
                    ReasonForRejection = e.Status == Domain.Enums.EventStatus.Rejected ? e.Approval.Reason : null
                }) ?? throw new NotFoundException($"Không tìm thấy sự kiện nào với id {id}");
        }
    }
}
