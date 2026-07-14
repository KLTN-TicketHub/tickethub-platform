using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Finance.Common.Dtos.Payouts;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers.V1.Moderator
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/moderator/payouts")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Moderator)]
    public class PayoutController : ControllerBase
    {
        private readonly IPayoutService _payoutService;
        private readonly ICurrentUserService _currentUserService;

        public PayoutController(IPayoutService payoutService, ICurrentUserService currentUserService)
        {
            _payoutService = payoutService;
            _currentUserService = currentUserService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetEventsPendingPayoutAsync(CancellationToken cancellationToken)
        {
            List<EventPendingPayoutDto> result = await _payoutService.GetEventsPendingPayoutAsync(cancellationToken);

            return Ok(new ApiResponse<List<EventPendingPayoutDto>>(true, "Lấy danh sách sự kiện chờ giải ngân thành công.", result));
        }

        [HttpPost("events/{eventId:guid}/release")]
        public async Task<IActionResult> ReleaseEventFundsAsync(
            [FromRoute] Guid eventId,
            [FromBody] ReleaseEventFundsRequestDto request,
            CancellationToken cancellationToken)
        {
            Guid reviewerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người duyệt.");

            (bool isSuccess, string message, EventPayoutResultDto? data) = await _payoutService.ReleaseEventFundsAsync(
                eventId, request.AppliedRate, reviewerId, _currentUserService.UserName, cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<EventPayoutResultDto>(true, message, data!));
        }
    }
}
