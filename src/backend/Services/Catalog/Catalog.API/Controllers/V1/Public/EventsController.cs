using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.EventClicks.Commands.TrackEventClick;
using Catalog.Application.Features.Events.Queries.GetEventById;
using Catalog.Application.Features.Events.Queries.GetEvents;
using Catalog.Application.Features.Events.Requests;
using Catalog.Domain.Enums;
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
        private readonly ICurrentUserService _currentUserService;

        public EventsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
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
            var result = await _sender.Send(new GetEventByIdQuery(eventId), cancellationToken);

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                Message = "Lấy chi tiết sự kiện thành công",
                Data = result,
            });
        }

        [AllowAnonymous]
        [HttpPost("{eventId:guid}/click/{clickType}")]
        public async Task<IActionResult> TrackEventClickAsync(Guid eventId, EventClickType clickType, CancellationToken cancellationToken = default)
        {
            await _sender.Send(new TrackEventClickCommand(eventId, _currentUserService.UserId, clickType), cancellationToken);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đã ghi nhận lượt tương tác.",
            });
        }
    }
}
