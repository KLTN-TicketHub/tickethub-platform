using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Queries.GetEvents;
using Catalog.Application.Features.Events.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ISender _sender;

        public EventsController(ISender sender)
        {
            _sender = sender;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEventsAsync([FromQuery] GetEventsRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetEventsQuery(request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<EventListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách sự kiện thành công",
                Data = result,
            });
        }

        [AllowAnonymous]
        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
