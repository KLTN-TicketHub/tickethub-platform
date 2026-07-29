using AutoMapper;
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

        public GetUserByIdQueryHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
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

            return result;
        }
    }
}
