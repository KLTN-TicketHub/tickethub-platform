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
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            await _payoutService.RequestPayoutAsync(eventId, organizerId, cancellationToken);

            return Ok(new ApiResponse(true, "Đã gửi yêu cầu giải ngân thành công. Moderator sẽ xem xét và phản hồi sớm."));
        }

        [HttpGet("proposed")]
        public async Task<IActionResult> GetProposedPayoutsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

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
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            EventPayoutResultDto data = await _payoutService.AcceptPayoutAsync(
                payoutId, organizerId, cancellationToken);

            return Ok(new ApiResponse<EventPayoutResultDto>(true, "Đã chấp nhận giải ngân, số dư ví đã được cập nhật.", data));
        }

        [HttpPost("{payoutId:guid}/reject")]
        public async Task<IActionResult> RejectPayoutAsync(
            [FromRoute] Guid payoutId,
            [FromBody] RejectPayoutRequestDto request,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            await _payoutService.RejectPayoutAsync(
                payoutId, organizerId, request.Reason, cancellationToken);

            return Ok(new ApiResponse(true, "Đã từ chối đề xuất giải ngân. Yêu cầu đã được chuyển lại cho Moderator xem xét."));
        }
    }
}
