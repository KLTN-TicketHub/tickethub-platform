using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Finance.Common.Dtos.Payouts;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/payouts")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class PayoutController : ControllerBase
    {
        private readonly IPayoutService _payoutService;
        private readonly ICurrentUserService _currentUserService;

        public PayoutController(IPayoutService payoutService, ICurrentUserService currentUserService)
        {
            _payoutService = payoutService;
            _currentUserService = currentUserService;
        }

        [HttpPost("events/{eventId:guid}/request")]
        public async Task<IActionResult> RequestPayoutAsync(
            [FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message) = await _payoutService.RequestPayoutAsync(eventId, organizerId, cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse(true, message));
        }

        [HttpGet("proposed")]
        public async Task<IActionResult> GetProposedPayoutsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            PaginatedResult<ProposedPayoutDto> result = await _payoutService.GetProposedPayoutsAsync(
                organizerId,
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<ProposedPayoutDto>>(true, "Lấy danh sách đề xuất giải ngân thành công.", result));
        }

        [HttpPost("{payoutId:guid}/accept")]
        public async Task<IActionResult> AcceptPayoutAsync(
            [FromRoute] Guid payoutId,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (bool isSuccess, string message, EventPayoutResultDto? data) = await _payoutService.AcceptPayoutAsync(
                payoutId, organizerId, cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<EventPayoutResultDto>(true, message, data!));
        }
    }
}
