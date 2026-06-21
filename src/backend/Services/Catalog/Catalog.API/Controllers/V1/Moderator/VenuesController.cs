using BuildingBlocks.API.Extensions;
using BuildingBlocks.Application.Interfaces;
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

namespace Catalog.API.Controllers.V1.Moderator
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/moderator/venues")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Moderator)]
    public class VenuesController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;

        public VenuesController(ISender sender, ICacheService cacheService)
        {
            _sender = sender;
            _cacheService = cacheService;
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

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetVenueByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"catalog:venue:id:{id}";

            VenueDto? result = await _cacheService.GetAsync<VenueDto>(cacheKey, cancellationToken);
            if (result == null)
            {
                result = await _sender.Send(new GetVenueByIdQuery(id), cancellationToken);
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10), cancellationToken);
            }

            return Ok(new ApiResponse<VenueDto>
            {
                Success = true,
                Message = "Lấy chi tiết địa điểm thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
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
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateVenueAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateVenueRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateVenueCommand(id, request), cancellationToken);

            // Invalidate specific venue details cache
            await _cacheService.RemoveAsync($"catalog:venue:id:{id}", cancellationToken);

            return Ok(new ApiResponse<VenueDto>
            {
                Success = true,
                Message = "Cập nhật địa điểm thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteVenueAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new DeleteVenueCommand(id), cancellationToken);

            // Invalidate specific venue details cache
            await _cacheService.RemoveAsync($"catalog:venue:id:{id}", cancellationToken);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa địa điểm thành công"
            });
        }
    }
}
