using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Commands.CreateVenue;
using Catalog.Application.Features.Venues.Commands.DeleteVenue;
using Catalog.Application.Features.Venues.Commands.UpdateVenue;
using Catalog.Application.Features.Venues.Queries.GetVenueById;
using Catalog.Application.Features.Venues.Queries.GetVenues;
using Catalog.Application.Features.Venues.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VenuesController : ControllerBase
    {
        private readonly ISender _sender;

        public VenuesController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Moderator)]
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

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetVenueByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            VenueDto result = await _sender.Send(new GetVenueByIdQuery(id), cancellationToken);

            return Ok(new ApiResponse<VenueDto>
            {
                Success = true,
                Message = "Lấy chi tiết địa điểm thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Moderator)]
        [HttpPost]
        public async Task<IActionResult> CreateVenueAsync([FromBody] CreateVenueRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateVenueCommand(request), cancellationToken);

            return Ok(new ApiResponse<VenueDto>
            {
                Success = true,
                Message = "Tạo địa điểm thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Moderator)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateVenueAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateVenueRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateVenueCommand(id, request), cancellationToken);

            return Ok(new ApiResponse<VenueDto>
            {
                Success = true,
                Message = "Cập nhật địa điểm thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Moderator)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteVenueAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new DeleteVenueCommand(id), cancellationToken);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa địa điểm thành công"
            });
        }
    }
}
