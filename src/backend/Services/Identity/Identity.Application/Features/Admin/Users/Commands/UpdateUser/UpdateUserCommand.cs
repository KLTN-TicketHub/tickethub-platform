using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Requests;
using MediatR;

namespace Identity.Application.Features.Admin.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(Guid Id, UpdateUserRequest Request) : IRequest<UserDetailDto>;
}
