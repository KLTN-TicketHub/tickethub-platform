using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Features.EventCancellationRequests.Commands.RequestEventCancellation;
using Catalog.Application.Features.EventCancellationRequests.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/event-cancellation-requests")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class EventCancellationRequestsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public EventCancellationRequestsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [HttpPost("events/{eventId:guid}")]
        public async Task<IActionResult> RequestEventCancellationAsync(
            [FromRoute] Guid eventId,
            [FromBody] RequestEventCancellationRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            var result = await _sender.Send(new RequestEventCancellationCommand(eventId, request, organizerId), cancellationToken);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Đã gửi yêu cầu hủy sự kiện thành công. Moderator sẽ xem xét và phản hồi sớm.",
                Data = result
            });
        }
    }
}
