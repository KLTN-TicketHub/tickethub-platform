using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Queries.GetEventClickTrend;
using Catalog.Application.Features.EventClicks.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/events/{eventId:guid}/click-trend")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class EventClickStatsController : ControllerBase
    {
        private readonly ISender _sender;

        public EventClickStatsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventClickTrendAsync(Guid eventId, [FromQuery] GetClickTrendRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetEventClickTrendQuery(eventId, request), cancellationToken);

            return Ok(new ApiResponse<List<ClickTrendPointDto>>
            {
                Success = true,
                Message = "Lấy xu hướng tương tác sự kiện thành công.",
                Data = result,
            });
        }
    }
}
