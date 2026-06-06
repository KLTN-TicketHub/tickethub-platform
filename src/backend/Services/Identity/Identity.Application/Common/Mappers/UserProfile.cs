using AutoMapper;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Common.DTOs.Auth;
using Identity.Domain.Entities;

namespace Identity.Application.Common.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ProfileDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.CreateAt,
                opt => opt.MapFrom(src => src.CreateAt));

            CreateMap<User, ModeratorDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreateAt));
        }
    }
}
