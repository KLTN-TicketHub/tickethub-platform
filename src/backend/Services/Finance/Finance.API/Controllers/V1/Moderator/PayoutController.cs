using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
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

        [HttpGet("requests")]
        public async Task<IActionResult> GetPayoutRequestsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            PaginatedResult<PayoutRequestDto> result = await _payoutService.GetPayoutRequestsAsync(
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<PayoutRequestDto>>(true, "Lấy danh sách yêu cầu giải ngân thành công.", result));
        }

        [HttpPost("requests/{payoutRequestId:guid}/propose")]
        public async Task<IActionResult> ProposePayoutAsync(
            [FromRoute] Guid payoutRequestId,
            [FromBody] ProposePayoutRequestDto request,
            CancellationToken cancellationToken)
        {
            Guid reviewerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người duyệt.");

            (bool isSuccess, string message, EventPayoutResultDto? data) = await _payoutService.ProposePayoutAsync(
                payoutRequestId, request.AppliedRate, reviewerId, _currentUserService.UserName, cancellationToken);

            if (!isSuccess)
                return BadRequest(new ApiResponse(false, message));

            return Ok(new ApiResponse<EventPayoutResultDto>(true, message, data!));
        }
    }
}
