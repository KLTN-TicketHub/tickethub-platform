using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Queries.GetOrganizerInsights;
using Catalog.Application.Features.EventClicks.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/insights")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class OrganizerInsightsController : ControllerBase
    {
        private readonly ISender _sender;

        public OrganizerInsightsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizerInsightsAsync([FromQuery] GetClickTrendRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetOrganizerInsightsQuery(request), cancellationToken);

            return Ok(new ApiResponse<OrganizerInsightsDto>
            {
                Success = true,
                Message = "Lấy thống kê tổng quan thành công.",
                Data = result,
            });
        }
    }
}
