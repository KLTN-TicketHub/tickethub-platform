using BuildingBlocks.Contracts.Models.Pagination;
using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Requests;
using MediatR;

namespace Identity.Application.Features.Admin.Users.Queries.GetUsers
{
    public record GetUsersQuery(GetUsersRequest Request) : IRequest<PaginatedResult<UserListItemDto>>;
}
