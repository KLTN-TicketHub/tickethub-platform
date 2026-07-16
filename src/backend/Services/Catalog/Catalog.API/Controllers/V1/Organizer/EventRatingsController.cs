using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Queries.GetEventRatingsForOrganizer;
using Catalog.Application.Features.EventRatings.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/events/{eventId:guid}/ratings")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class EventRatingsController : ControllerBase
    {
        private readonly ISender _sender;

        public EventRatingsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventRatingsForOrganizerAsync(Guid eventId, [FromQuery] GetEventRatingsRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetEventRatingsForOrganizerQuery(eventId, request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<EventRatingDto>>
            {
                Success = true,
                Message = "Lấy danh sách đánh giá thành công.",
                Data = result,
            });
        }
    }
}
