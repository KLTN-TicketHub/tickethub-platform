using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Extensions;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace Catalog.Application.Features.SeatMaps.Queries.GetSeatMapById
{
    public class GetSeatMapByIdQueryHandler : IRequestHandler<GetSeatMapByIdQuery, SeatMapDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetSeatMapByIdQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<SeatMapDto> Handle(GetSeatMapByIdQuery request, CancellationToken cancellationToken)
        {
            return await GetSeatMapByIdAsync(request.VenueId, request.Id, cancellationToken);
        }

        private async Task<SeatMapDto> GetSeatMapByIdAsync(Guid venueId, Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.SeatMapRepository.GetOneUntrackedAsync(
                filter: x => x.VenueId == venueId && x.Id == id && !x.IsDeleted && !x.Venue.IsDeleted,
                selector: x => new SeatMapDto
                {
                    Id = id,
                    VenueId = venueId,
                    SeatMapName = x.SeatMapName,
                    SeatMapCode = x.SeatMapCode,
                    Width = x.Width,
                    Height = x.Height,
                    SvgFileUrl = x.SvgFileUrl != null ? _fileService.GetAbsoluteUrl(x.SvgFileUrl) : null,
                    RowVersion = x.RowVersion,
                    Zones = x.Zones.Select(z => new ZoneDto
                    {
                        Id = z.Id,
                        ZoneName = z.ZoneName,
                        ZoneCode = z.ZoneCode,
                        Color = z.Color,
                        X = z.X,
                        Y = z.Y,
                        Width = z.Width,
                        Height = z.Height,
                        IsStage = z.IsStage,
                        IsReservingSeat = z.IsReservingSeat,
                        IsSalable = z.IsSalable,
                        SvgElementId = z.SvgElementId,
                        Capacity = z.Capacity,
                        BasePrice = z.BasePrice,
                        DisplayOrder = z.DisplayOrder,
                        SvgElements = z.ElementJson != null ?
                            JsonSerializer.Deserialize<List<SvgElementDto>>(z.ElementJson) ?? new List<SvgElementDto>() : null,
                        Rows = z.Rows.Select(r => new RowDto
                        {
                            Id = r.Id,
                            RowName = r.RowName,
                            RowVersion = r.RowVersion,
                            Seats = r.Seats.Select(s => new SeatDto
                            {
                                Id = s.Id,
                                SeatName = s.SeatName,
                                SeatCode = s.SeatCode,
                                SvgElementId = s.SvgElementId,
                                X = s.X,
                                Y = s.Y,
                                Radius = s.Radius,
                                LayoutStatus = s.LayoutStatus.GetDisplayName(),
                                RowVersion = s.RowVersion
                            }).ToList()
                        }).ToList()
                    }).ToList()
                },
                cancellation: cancellationToken) ?? throw new BuildingBlocks.Domain.Exceptions.NotFoundException("Không tìm thấy bản đồ ghế.");
        }
    }
}
