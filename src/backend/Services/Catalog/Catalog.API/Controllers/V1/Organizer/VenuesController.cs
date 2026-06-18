using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Queries.GetVenues;
using Catalog.Application.Features.Venues.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/venues")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class VenuesController : ControllerBase
    {
        private readonly ISender _sender;

        public VenuesController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet]
        public async Task<IActionResult> GetVenuesAsync(
            [FromQuery] GetVenuesRequest request,
            CancellationToken cancellationToken = default)
        {
            PaginatedResult<VenueListItemDto> result = await _sender.Send(new GetVenuesQuery(request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<VenueListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách địa điểm thành công",
                Data = result
            });
        }
    }
}
