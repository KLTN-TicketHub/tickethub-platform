using AutoMapper;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Admin.Users.Requests;
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
                opt => opt.MapFrom(src => src.CreatedAt));

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
                opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, StaffDto>()
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
                opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, StaffListItemDto>()
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
                .ForMember(dest => dest.EmailConfirmed,
                opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsLocked,
                opt => opt.Ignore());

            CreateMap<User, OrganizerDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.OrganizerName,
                opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<User, UserListItemDto>()
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
                .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.EmailConfirmed,
                opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Roles,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsLocked,
                opt => opt.Ignore());

            CreateMap<User, UserDetailDto>()
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
                .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.EmailConfirmed,
                opt => opt.MapFrom(src => src.EmailConfirmed))
                .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Roles,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsLocked,
                opt => opt.Ignore());

            CreateMap<UpdateUserRequest, User>()
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl));
        }
    }
}
