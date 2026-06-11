using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Catalog.Application.Features.SeatMaps.Commands.CreateSeatMap
{
    public class CreateSeatMapCommandHandler : IRequestHandler<CreateSeatMapCommand, SeatMapDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CreateSeatMapCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }


        public async Task<SeatMapDto> Handle(CreateSeatMapCommand command, CancellationToken cancellation = default)
        {
            return await CreateSeatMapAsync(command.VenueId, command.Request, cancellation);
        }

        private async Task<SeatMapDto> CreateSeatMapAsync(Guid venueId, CreateSeatMapRequest request, CancellationToken cancellation = default)
        {
            Venue? venue = await _unitOfWork.VenueRepository.GetOneUntrackedAsync<Venue>(
                filter: v => v.Id == venueId && !v.IsDeleted,
                include: v => v.Include(v => v.SeatMaps.Where(sm => sm.SeatMapName == request.SeatMapName && !sm.IsDeleted)),
                cancellation: cancellation)
                    ?? throw new NotFoundException($"Không tìm thấy địa điểm với ID {venueId}");

            if (venue.SeatMaps.Any())
                throw new ValidatorException(nameof(request.SeatMapName), $"Đã tồn tại sơ đồ chỗ ngồi với tên '{request.SeatMapName}' trong địa điểm này.");

            string seatMapCode = await _unitOfWork
                .SeatMapRepository
                .GenerateNextSeatMapCodeAsync(venueId, request.SeatMapName, cancellation);

            int tempDisplayOrder = 0;
            request.Zones = request.Zones.OrderBy(z => z.DisplayOrder).ToList();
            request.Zones.ForEach(z =>
            {
                z.DisplayOrder = tempDisplayOrder++;
            });

            SeatMap seatMap = _mapper.Map<SeatMap>(request);
            seatMap.VenueId = venueId;
            seatMap.SetSeatMapCode(seatMapCode);

            if (!string.IsNullOrWhiteSpace(request.SvgFileUrl))
            {
                if (!_fileService.FileExists(request.SvgFileUrl))
                    throw new NotFoundException($"Không tìm thấy tệp SVG tại đường dẫn '{request.SvgFileUrl}'.");

                seatMap.SvgFileUrl = request.SvgFileUrl;
            }

            if (request.Zones is { Count: > 0 })
            {
                foreach (var zoneRequest in request.Zones)
                {
                    Zone zone = BuildZone(zoneRequest, seatMap.Id, seatMapCode);
                    seatMap.AddZone(zone);
                }
            }

            return _mapper.Map<SeatMapDto>(
                await _unitOfWork.SeatMapRepository.CreateAsync(seatMap, cancellation));
        }

        private Zone BuildZone(
            CreateZoneRequest zoneRequest,
            Guid seatMapId,
            string seatMapCode)
        {
            string zoneCode = Zone.NormalizeZoneCode(zoneRequest.ZoneName);
            zoneCode = $"{seatMapCode}-{zoneCode}";

            Zone zone = _mapper.Map<Zone>(zoneRequest);
            zone.SetZoneCode(zoneCode);

            if (zoneRequest.SvgElements is { Count: > 0 })
            {
                zone.SetElementJson(JsonSerializer.Serialize(zoneRequest.SvgElements));
            }

            if (zoneRequest.Rows is { Count: > 0 })
            {
                foreach (var rowRequest in zoneRequest.Rows)
                {
                    Row row = BuildRow(rowRequest, zoneCode);
                    zone.AddRow(row);
                }
            }

            return zone;
        }

        private Row BuildRow(CreateRowRequest rowRequest, string zoneCode)
        {
            Row row = _mapper.Map<Row>(rowRequest);

            if (rowRequest.SeatRequests is { Count: > 0 })
            {
                foreach (var seatRequest in rowRequest.SeatRequests)
                {
                    Seat seat = BuildSeat(seatRequest, row.RowName, zoneCode);
                    row.AddSeat(seat);
                }
            }

            return row;
        }

        private Seat BuildSeat(CreateSeatRequest seatRequest, string rowName, string zoneCode)
        {
            string seatCode = Seat.NormalizeSeatCode(seatRequest.SeatName);
            seatCode = $"{zoneCode}-{rowName}-{seatCode}";

            Seat seat = _mapper.Map<Seat>(seatRequest);
            seat.SetSeatCode(seatCode);

            return seat;
        }
    }
}
