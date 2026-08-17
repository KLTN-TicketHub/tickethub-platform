using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Finance.Common.Dtos.Wallets;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers.V1.Organizer
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizer/wallet")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Organizer)]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ICurrentUserService _currentUserService;

        public WalletController(IWalletService walletService, ICurrentUserService currentUserService)
        {
            _walletService = walletService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletAsync(CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            WalletDto result = await _walletService.GetWalletAsync(organizerId, cancellationToken);

            return Ok(new ApiResponse<WalletDto>(true, "Lấy thông tin ví thành công.", result));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetWalletTransactionsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            PaginatedResult<WalletTransactionDto> result = await _walletService.GetWalletTransactionsAsync(
                organizerId,
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<WalletTransactionDto>>(true, "Lấy lịch sử giao dịch ví thành công.", result));
        }
    }
}
