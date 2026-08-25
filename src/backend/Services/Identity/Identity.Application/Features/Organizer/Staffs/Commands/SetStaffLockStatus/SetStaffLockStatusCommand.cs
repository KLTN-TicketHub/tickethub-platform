using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Requests;
using MediatR;

namespace Identity.Application.Features.Organizer.Staffs.Commands.SetStaffLockStatus
{
    public record SetStaffLockStatusCommand(Guid OrganizerId, Guid StaffId, SetStaffLockStatusRequest Request) : IRequest<StaffListItemDto>;
}
