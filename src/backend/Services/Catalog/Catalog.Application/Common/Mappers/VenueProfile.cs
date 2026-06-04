using AutoMapper;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Mappers
{
    public class VenueProfile : Profile
    {
        public VenueProfile()
        {
            CreateMap<CreateVenueRequest, Venue>()
                .ConstructUsing(src => new Venue(
                    src.VenueName,
                    src.AddressLine,
                    src.Ward,
                    src.District,
                    src.ProvinceCity,
                    src.Country,
                    src.Longitude,
                    src.Latitude,
                    src.PhoneNumber,
                    src.WebsiteUrl
                ));

            CreateMap<Venue, VenueDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VenueName,
                opt => opt.MapFrom(src => src.VenueName))
                .ForMember(dest => dest.VenueCode,
                opt => opt.MapFrom(src => src.VenueCode))
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
                .ForMember(dest => dest.Slug,
                opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Longitude,
                opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Latitude,
                opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.WebsiteUrl,
                opt => opt.MapFrom(src => src.WebsiteUrl))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RowVersion,
                opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<UpdateVenueRequest, Venue>()
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
                .ForMember(dest => dest.Longitude,
                opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Latitude,
                opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.WebsiteUrl,
                opt => opt.MapFrom(src => src.WebsiteUrl));
        }
    }
}
