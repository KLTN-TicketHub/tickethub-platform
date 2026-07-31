using Identity.Application.Common.DTOs.Admin;
using MediatR;

namespace Identity.Application.Features.Admin.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<UserDetailDto>;
}
