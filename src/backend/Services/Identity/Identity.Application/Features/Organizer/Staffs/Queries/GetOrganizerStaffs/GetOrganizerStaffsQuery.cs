using BuildingBlocks.Contracts.Models.Pagination;
using Identity.Application.Common.DTOs.Organizer;
using Identity.Application.Features.Organizer.Staffs.Requests;
using MediatR;

namespace Identity.Application.Features.Organizer.Staffs.Queries.GetOrganizerStaffs
{
    public record GetOrganizerStaffsQuery(Guid OrganizerId, GetOrganizerStaffsRequest Request) : IRequest<PaginatedResult<StaffListItemDto>>;
}
