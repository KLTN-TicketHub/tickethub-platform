using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventCancellationRequests;
using Catalog.Application.Features.EventCancellationRequests.Commands.ReviewEventCancellationRequest;
using Catalog.Application.Features.EventCancellationRequests.Queries.GetEventCancellationRequests;
using Catalog.Application.Features.EventCancellationRequests.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Moderator
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/moderator/event-cancellation-requests")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Moderator)]
    public class EventCancellationRequestsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public EventCancellationRequestsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventCancellationRequestsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            PaginatedResult<EventCancellationRequestDto> result = await _sender.Send(
                new GetEventCancellationRequestsQuery(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 12 : pageSize),
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<EventCancellationRequestDto>>
            {
                Success = true,
                Message = "Lấy danh sách yêu cầu hủy sự kiện thành công.",
                Data = result
            });
        }

        [HttpPost("{requestId:guid}/review")]
        public async Task<IActionResult> ReviewEventCancellationRequestAsync(
            [FromRoute] Guid requestId,
            [FromBody] ReviewEventCancellationRequestRequest request,
            CancellationToken cancellationToken = default)
        {
            Guid reviewerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người duyệt.");

            var result = await _sender.Send(
                new ReviewEventCancellationRequestCommand(requestId, request, reviewerId, _currentUserService.UserName),
                cancellationToken);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = request.IsApproved ? "Đã duyệt yêu cầu hủy sự kiện thành công." : "Đã từ chối yêu cầu hủy sự kiện thành công.",
                Data = result
            });
        }
    }
}
