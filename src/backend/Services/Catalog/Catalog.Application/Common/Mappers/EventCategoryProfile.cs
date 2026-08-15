using AutoMapper;
using Catalog.Application.Common.DTOs.EventCategories;
using Catalog.Application.Features.EventCategories.Requests;
using Catalog.Domain.Entities;

namespace Catalog.Application.Common.Mappers
{
    public class EventCategoryProfile : Profile
    {
        public EventCategoryProfile()
        {
            CreateMap<CreateEventCategoryRequest, EventCategory>()
                .ConstructUsing(src => new EventCategory(
                    src.CategoryName,
                    src.Description,
                    src.RecommendedCommissionRate
                ))
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore());

            CreateMap<EventCategory, EventCategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryCode, opt => opt.MapFrom(src => src.CategoryCode))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.RecommendedCommissionRate, opt => opt.MapFrom(src => src.RecommendedCommissionRate))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<UpdateEventCategoryRequest, EventCategory>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.RecommendedCommissionRate, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore());
        }
    }
}
