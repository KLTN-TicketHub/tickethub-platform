using AutoMapper;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Mappers
{
    public class EventRatingProfile : Profile
    {
        public EventRatingProfile()
        {
            CreateMap<EventRating, EventRatingDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EventId,
                opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ReviewerName,
                opt => opt.MapFrom(src => src.ReviewerName))
                .ForMember(dest => dest.SoundRating,
                opt => opt.MapFrom(src => src.SoundRating))
                .ForMember(dest => dest.VisualRating,
                opt => opt.MapFrom(src => src.VisualRating))
                .ForMember(dest => dest.OrganizationRating,
                opt => opt.MapFrom(src => src.OrganizationRating))
                .ForMember(dest => dest.FacilityRating,
                opt => opt.MapFrom(src => src.FacilityRating))
                .ForMember(dest => dest.ServiceRating,
                opt => opt.MapFrom(src => src.ServiceRating))
                .ForMember(dest => dest.PerformanceRating,
                opt => opt.MapFrom(src => src.PerformanceRating))
                .ForMember(dest => dest.OverallRating,
                opt => opt.MapFrom(src => src.OverallRating))
                .ForMember(dest => dest.Comment,
                opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
