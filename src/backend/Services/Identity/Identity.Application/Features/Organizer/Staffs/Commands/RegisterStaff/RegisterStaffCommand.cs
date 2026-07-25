using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Requests;
using MediatR;

namespace Identity.Application.Features.Organizer.Staffs.Commands.RegisterStaff
{
    public record RegisterStaffCommand(Guid OrganizerId, RegisterStaffRequest Request) : IRequest<StaffDto>;
}
