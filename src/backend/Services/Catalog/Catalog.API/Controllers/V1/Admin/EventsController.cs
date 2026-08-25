using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Queries.GetAdminEvents;
using Catalog.Application.Features.Events.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/events")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class EventsController : ControllerBase
    {
        private readonly ISender _sender;

        public EventsController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet]
        public async Task<IActionResult> GetAdminEventsAsync(
            [FromQuery] GetAdminEventsRequest request,
            CancellationToken cancellationToken = default)
        {
            PaginatedResult<AdminEventListItemDto> result = await _sender.Send(new GetAdminEventsQuery(request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<AdminEventListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách sự kiện thành công",
                Data = result
            });
        }
    }
}
