using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Commands.CreateVenue;
using Catalog.Application.Features.Venues.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StackExchange.Redis;

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
        [AllowAnonymous]
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
    }
}
