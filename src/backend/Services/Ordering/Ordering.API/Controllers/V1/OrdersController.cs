using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Ordering.Common.Dtos;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Interfaces.IServices;
using System.Security.Claims;

namespace Ordering.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly OrderingDbContext _dbContext;

        public OrdersController(IOrderService orderService, OrderingDbContext dbContext)
        {
            _orderService = orderService;
            _dbContext = dbContext;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string? emailFromJwt =
                User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("emailaddress")?.Value;

            request.CustomerEmail = emailFromJwt ?? string.Empty;

            (bool isSuccess, Guid orderId, string message) = await _orderService.CheckoutAsync(request, Guid.Parse(userIdClaim!));

            if (!isSuccess)
            {
                return BadRequest(new ApiResponse(false, message));
            }

            return Ok(new ApiResponse<Guid>(true, message, orderId));
        }

        [HttpGet("{orderId}/payment-link")]
        public async Task<IActionResult> GetPaymentLink(Guid orderId)
        {
            var sagaState = await _dbContext.OrderBookingStates
                .FirstOrDefaultAsync(x => x.CorrelationId == orderId);

            if (sagaState == null)
            {
                return NotFound(new ApiResponse(false, "Không tìm thấy thông tin thanh toán cho đơn hàng này."));
            }

            if (string.IsNullOrEmpty(sagaState.PaymentLink))
            {
                if (sagaState.CurrentState == "Failed")
                {
                    return BadRequest(new ApiResponse(false, "Đơn hàng đã bị hủy hoặc hết hạn thanh toán."));
                }

                return Ok(new ApiResponse<object>(true, "Đang khởi tạo liên kết thanh toán, vui lòng đợi...", new
                {
                    status = sagaState.CurrentState,
                    paymentLink = (string?)null
                }));
            }

            return Ok(new ApiResponse<object>(true, "Lấy liên kết thanh toán thành công.", new
            {
                status = sagaState.CurrentState,
                paymentLink = sagaState.PaymentLink
            }));
        }
    }
}
