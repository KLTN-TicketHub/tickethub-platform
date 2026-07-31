using AutoMapper;
using BuildingBlocks.Contracts.Models.Pagination;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Identity.Application.Features.Admin.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<UserListItemDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            return await GetUsersAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<UserListItemDto>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
        {
            List<Guid>? roleUserIds = await GetRoleUserIdsAsync(request.Role);

            Expression<Func<User, bool>> filter = BuildFilter(request, roleUserIds);

            (IEnumerable<User> users, int totalCount) = await _unitOfWork.UserRepository.GetPagedAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(u => u.CreatedAt),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            List<UserListItemDto> items = new List<UserListItemDto>();
            foreach (User user in users)
            {
                items.Add(await MapUserAsync(user));
            }

            return new PaginatedResult<UserListItemDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        private async Task<List<Guid>?> GetRoleUserIdsAsync(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return null;

            IList<User> usersInRole = await _userManager.GetUsersInRoleAsync(role);
            return usersInRole.Select(u => u.Id).ToList();
        }

        private static Expression<Func<User, bool>> BuildFilter(GetUsersRequest request, List<Guid>? roleUserIds)
        {
            string? search = request.Search?.Trim();
            bool hasSearch = !string.IsNullOrWhiteSpace(search);
            bool? isLocked = request.IsLocked;
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return u =>
                (!hasSearch
                    || u.FullName.Contains(search!)
                    || (u.Email != null && u.Email.Contains(search!))
                    || (u.UserName != null && u.UserName.Contains(search!)))
                && (roleUserIds == null || roleUserIds.Contains(u.Id))
                && (isLocked == null
                    || (isLocked.Value
                        ? (u.LockoutEnabled && u.LockoutEnd != null && u.LockoutEnd > now)
                        : (!u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd <= now)));
        }

        private async Task<UserListItemDto> MapUserAsync(User user)
        {
            UserListItemDto dto = _mapper.Map<UserListItemDto>(user);
            dto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            dto.IsLocked = await _userManager.IsLockedOutAsync(user);

            return dto;
        }
    }
}
