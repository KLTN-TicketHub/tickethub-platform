using AutoMapper;
using BuildingBlocks.Contracts.Extensions;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Requests;
using Catalog.Domain.Entities;
using System.Text.Json;

namespace Catalog.Application.Common.Mappers
{
    public class SeatMapProfile : Profile
    {
        public SeatMapProfile()
        {
            CreateMap<SeatMap, SeatMapDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VenueId,
                opt => opt.MapFrom(src => src.VenueId))
                .ForMember(dest => dest.SeatMapName,
                opt => opt.MapFrom(src => src.SeatMapName))
                .ForMember(dest => dest.SeatMapCode,
                opt => opt.MapFrom(src => src.SeatMapCode))
                .ForMember(dest => dest.SvgFileUrl,
                opt => opt.MapFrom(src => src.SvgFileUrl))
                .ForMember(dest => dest.Width,
                opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height,
                opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Zones,
                opt => opt.MapFrom(src => src.Zones))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<Zone, ZoneDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ZoneName,
                opt => opt.MapFrom(src => src.ZoneName))
                .ForMember(dest => dest.ZoneCode,
                opt => opt.MapFrom(src => src.ZoneCode))
                .ForMember(dest => dest.Width,
                opt => opt.MapFrom(src => src.Width))
                .ForMember(dest => dest.Height,
                opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.X,
                opt => opt.MapFrom(src => src.X))
                .ForMember(dest => dest.Y,
                opt => opt.MapFrom(src => src.Y))
                .ForMember(dest => dest.Color,
                opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.IsStage,
                opt => opt.MapFrom(src => src.IsStage))
                .ForMember(dest => dest.IsReservingSeat,
                opt => opt.MapFrom(src => src.IsReservingSeat))
                .ForMember(dest => dest.IsSalable,
                opt => opt.MapFrom(src => src.IsSalable))
                .ForMember(dest => dest.SvgElementId,
                opt => opt.MapFrom(src => src.SvgElementId))
                .ForMember(dest => dest.Capacity,
                opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.BasePrice,
                opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.SvgElements,
                opt => opt.MapFrom(
                    src => src.ElementJson == null ?
                        null : JsonSerializer.Deserialize<List<SvgElementDto>>(src.ElementJson)))
                .ForMember(dest => dest.Rows,
                opt => opt.MapFrom(src => src.Rows))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<Row, RowDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RowName,
                opt => opt.MapFrom(src => src.RowName))
                .ForMember(dest => dest.Seats,
                opt => opt.MapFrom(src => src.Seats))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<Seat, SeatDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SeatName,
                opt => opt.MapFrom(src => src.SeatName))
                .ForMember(dest => dest.SeatCode,
                opt => opt.MapFrom(src => src.SeatCode))
                .ForMember(dest => dest.SvgElementId,
                opt => opt.MapFrom(src => src.SvgElementId))
                .ForMember(dest => dest.X,
                opt => opt.MapFrom(src => src.X))
                .ForMember(dest => dest.Y,
                opt => opt.MapFrom(src => src.Y))
                .ForMember(dest => dest.Radius,
                opt => opt.MapFrom(src => src.Radius))
                .ForMember(dest => dest.LayoutStatus,
                opt => opt.MapFrom(src => src.LayoutStatus.GetDisplayName()))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<CreateSeatMapRequest, SeatMap>()
                .ConstructUsing(src => new SeatMap(
                    src.VenueId,
                    src.SeatMapName,
                    src.Width,
                    src.Height))
                .ForMember(dest => dest.Zones,
                opt => opt.Ignore());

            CreateMap<CreateZoneRequest, Zone>()
                .ConstructUsing(src => new Zone(
                    src.ZoneName,
                    src.Color,
                    src.X,
                    src.Y,
                    src.Width,
                    src.Height,
                    src.IsStage,
                    src.IsReservingSeat,
                    src.IsSalable,
                    src.SvgElementId,
                    src.Capacity,
                    src.BasePrice,
                    src.DisplayOrder ?? 0))
                .ForMember(dest => dest.Rows,
                opt => opt.Ignore());

            CreateMap<CreateRowRequest, Row>()
                .ConstructUsing(src => new Row(
                    src.RowLabel))
                .ForMember(dest => dest.Seats,
                opt => opt.Ignore());

            CreateMap<CreateSeatRequest, Seat>()
                .ConstructUsing(src => new Seat(
                    src.SeatName,
                    src.SvgElementId,
                    src.X,
                    src.Y,
                    src.Radius));
        }
    }
}
