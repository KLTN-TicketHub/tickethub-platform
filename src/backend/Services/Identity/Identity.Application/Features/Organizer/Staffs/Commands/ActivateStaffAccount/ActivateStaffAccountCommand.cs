using Identity.Application.Features.Organizer.Staffs.Requests;
using MediatR;

namespace Identity.Application.Features.Organizer.Staffs.Commands.ActivateStaffAccount
{
    public record ActivateStaffAccountCommand(ActivateStaffAccountRequest Request) : IRequest<Unit>;
}
