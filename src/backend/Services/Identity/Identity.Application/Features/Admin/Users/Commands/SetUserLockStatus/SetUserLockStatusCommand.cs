using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Users.Requests;
using MediatR;

namespace Identity.Application.Features.Admin.Users.Commands.SetUserLockStatus
{
    public record SetUserLockStatusCommand(Guid Id, SetUserLockStatusRequest Request) : IRequest<UserDetailDto>;
}
