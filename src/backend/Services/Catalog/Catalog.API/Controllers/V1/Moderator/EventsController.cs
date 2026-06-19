using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Features.Events.Commands.ReviewEvent;
using Catalog.Application.Features.Events.Queries.GetEventsForModerator;
using Catalog.Application.Features.Events.Requests;
using Catalog.Application.Common.DTOs.Events;
using BuildingBlocks.Contracts.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Features.Events.Queries.GetByIdForModerator;

namespace Catalog.API.Controllers.V1.Moderator
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/moderator/events")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Moderator)]
    public class EventsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public EventsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [HttpPost("{id:guid}/review")]
        public async Task<IActionResult> ReviewEventAsync(
            [FromRoute] Guid id,
            [FromBody] ReviewEventRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid reviewerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người duyệt.");
            
            string? reviewerName = _currentUserService.UserName;

            var result = await _sender.Send(new ReviewEventCommand(id, request, reviewerId, reviewerName), cancellationToken);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = request.IsApproved ? "Đã duyệt sự kiện thành công." : "Đã từ chối sự kiện thành công.",
                Data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsAsync(
            [FromQuery] GetEventsForModeratorRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetEventsForModeratorQuery(request), cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<ModeratorEventListItemDto>>
            {
                Success = true,
                Message = "Lấy danh sách sự kiện thành công.",
                Data = result
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEventByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new GetByIdForModeratorQuery(id), cancellationToken);

            return Ok(new ApiResponse<EventDto>
            {
                Success = true,
                Message = "Lấy thông tin sự kiện thành công.",
                Data = result
            });
        }
    }
}
