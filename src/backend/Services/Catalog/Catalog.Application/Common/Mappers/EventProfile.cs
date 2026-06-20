using AutoMapper;
using BuildingBlocks.Contracts.Extensions;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Mappers
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<CreateEventRequest, Event>()
                .ConstructUsing(src => new Event(
                    Guid.Empty,
                    src.CategoryId,
                    src.Title,
                    src.Description,
                    src.ShowTimes.Any() ? src.ShowTimes.Min(s => s.StartAt) : DateTime.UtcNow,
                    src.ShowTimes.Any() ? src.ShowTimes.Max(s => s.EndAt) : DateTime.UtcNow,
                    src.SaleOpenAt,
                    src.SaleCloseAt,
                    src.CoverImageUrl,
                    "VND",
                    src.SeatMapId))
                .ForMember(dest => dest.Location,
                opt => opt.Ignore())
                .ForMember(dest => dest.ShowTimes,
                opt => opt.Ignore());

            CreateMap<CreateTicketTypeRequest, TicketType>()
                .ConstructUsing(src => new TicketType(
                    src.TicketTypeName,
                    src.Price,
                    src.PublishedQuota ?? 0,
                    src.MinQtyQuota,
                    src.MaxQtyQuota,
                    src.DisplayOrder,
                    src.Description,
                    src.ZoneId));

            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SeatMapId,
                opt => opt.MapFrom(src => src.SeatMapId))
                .ForMember(dest => dest.VenueId,
                opt => opt.MapFrom(src => src.SeatMap != null ? src.SeatMap.VenueId : (Guid?)null))
                .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Slug,
                opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.StartAt,
                opt => opt.MapFrom(src => src.StartAt))
                .ForMember(dest => dest.EndAt,
                opt => opt.MapFrom(src => src.EndAt))
                .ForMember(dest => dest.SaleOpenAt,
                opt => opt.MapFrom(src => src.SaleOpenAt))
                .ForMember(dest => dest.SaleCloseAt,
                opt => opt.MapFrom(src => src.SaleCloseAt))
                .ForMember(dest => dest.CurrencyCode,
                opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.CoverImageUrl,
                opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.GetDisplayName()))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.Location,
                opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Showtimes,
                opt => opt.MapFrom(src => src.ShowTimes));

            CreateMap<ShowTime, ShowtimeDto>()
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.GetDisplayName()))
                .ForMember(dest => dest.TicketTypes,
                opt => opt.MapFrom(src => src.TicketTypes));

            CreateMap<EventLocation, EventLocationDto>()
                .ForMember(dest => dest.VenueName,
                opt => opt.MapFrom(src => src.VenueName))
                .ForMember(dest => dest.AddressLine,
                opt => opt.MapFrom(src => src.AddressLine))
                .ForMember(dest => dest.Ward,
                opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District,
                opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.ProvinceCity,
                opt => opt.MapFrom(src => src.ProvinceCity))
                .ForMember(dest => dest.Country,
                opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<TicketType, TicketTypeDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ZoneId,
                opt => opt.MapFrom(src => src.ZoneId))
                .ForMember(dest => dest.TicketTypeName,
                opt => opt.MapFrom(src => src.TicketTypeName))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.PublishedQuota,
                opt => opt.MapFrom(src => src.PublishedQuota))
                .ForMember(dest => dest.MinQtyQuota,
                opt => opt.MapFrom(src => src.MinQtyQuota))
                .ForMember(dest => dest.MaxQtyQuota,
                opt => opt.MapFrom(src => src.MaxQtyQuota))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.GetDisplayName()))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));
        }
    }
}
