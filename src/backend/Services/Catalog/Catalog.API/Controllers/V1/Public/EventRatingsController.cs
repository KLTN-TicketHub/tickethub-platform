using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.EventRatings;
using Catalog.Application.Features.EventRatings.Commands.CreateEventRating;
using Catalog.Application.Features.EventRatings.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/events/{eventId:guid}/ratings")]
    [ApiController]
    public class EventRatingsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public EventRatingsController(ISender sender, ICurrentUserService currentUserService)
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public async Task<IActionResult> CreateEventRatingAsync(Guid eventId, [FromBody] CreateEventRatingRequest request, CancellationToken cancellationToken = default)
        {
            Guid userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");
            string reviewerName = _currentUserService.UserName ?? "Người dùng ẩn danh";

            var result = await _sender.Send(new CreateEventRatingCommand(eventId, userId, reviewerName, request), cancellationToken);

            return Ok(new ApiResponse<EventRatingDto>
            {
                Success = true,
                Message = "Đánh giá sự kiện thành công.",
                Data = result,
            });
        }
    }
}
