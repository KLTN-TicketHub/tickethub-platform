using AutoMapper;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Requests;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace Identity.Application.Features.Organizer.Staffs.Queries.GetOrganizerStaffs
{
    public class GetOrganizerStaffsQueryHandler : IRequestHandler<GetOrganizerStaffsQuery, PaginatedResult<StaffListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public GetOrganizerStaffsQueryHandler(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<StaffListItemDto>> Handle(GetOrganizerStaffsQuery query, CancellationToken cancellationToken)
        {
            return await GetOrganizerStaffsAsync(query.OrganizerId, query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<StaffListItemDto>> GetOrganizerStaffsAsync(
            Guid organizerId,
            GetOrganizerStaffsRequest request,
            CancellationToken cancellationToken)
        {
            IList<User> staffUsers = await _userManager.GetUsersInRoleAsync(Roles.Staff);
            List<Guid> staffUserIds = staffUsers.Select(u => u.Id).ToList();

            Expression<Func<User, bool>> filter = BuildFilter(organizerId, staffUserIds, request.Search);

            (IEnumerable<User> users, int totalCount) = await _unitOfWork.UserRepository.GetPagedAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(u => u.CreatedAt),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);

            List<StaffListItemDto> items = new List<StaffListItemDto>();
            foreach (User user in users)
            {
                items.Add(await MapStaffAsync(user));
            }

            return new PaginatedResult<StaffListItemDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        private static Expression<Func<User, bool>> BuildFilter(Guid organizerId, List<Guid> staffUserIds, string? search)
        {
            string? trimmedSearch = search?.Trim();
            bool hasSearch = !string.IsNullOrWhiteSpace(trimmedSearch);

            return u =>
                staffUserIds.Contains(u.Id) &&
                u.CreatedBy == organizerId &&
                (!hasSearch
                    || u.FullName.Contains(trimmedSearch!)
                    || (u.Email != null && u.Email.Contains(trimmedSearch!))
                    || (u.UserName != null && u.UserName.Contains(trimmedSearch!)));
        }

        private async Task<StaffListItemDto> MapStaffAsync(User user)
        {
            StaffListItemDto dto = _mapper.Map<StaffListItemDto>(user);
            dto.IsLocked = await _userManager.IsLockedOutAsync(user);

            return dto;
        }
    }
}
