using AutoMapper;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Identity.Application.Common.DTOs.Admin;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.Admin.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public GetUserByIdQueryHandler(UserManager<User> userManager, IMapper mapper, IFileService fileService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<UserDetailDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetUserByIdAsync(query.Id);
        }

        private async Task<UserDetailDto> GetUserByIdAsync(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString())
                ?? throw new NotFoundException($"Không tìm thấy người dùng với ID {id}.");

            UserDetailDto result = _mapper.Map<UserDetailDto>(user);
            result.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            result.IsLocked = await _userManager.IsLockedOutAsync(user);
            result.ImageUrl = ResolveImageUrl(user.ImageUrl);

            return result;
        }

        private string? ResolveImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://"))
                return imageUrl;

            return _fileService.GetAbsoluteUrl(imageUrl);
        }
    }
}
