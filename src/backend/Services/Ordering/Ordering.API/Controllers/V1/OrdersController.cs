using BuildingBlocks.Contracts.Models.Responses;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Ordering.Common.Dtos;
using Ordering.Common.Dtos.Reports;
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
        private readonly IReportService _reportService;
        private readonly OrderingDbContext _dbContext;

        public OrdersController(IOrderService orderService, IReportService reportService, OrderingDbContext dbContext)
        {
            _orderService = orderService;
            _reportService = reportService;
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

            Guid orderId = await _orderService.CheckoutAsync(request, Guid.Parse(userIdClaim!));

            return Ok(new ApiResponse<Guid>(true, "Đơn hàng đã được tạo thành công.", orderId));
        }

        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingOrder([FromQuery] Guid showtimeId)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            PendingOrderDto? pendingOrder = await _orderService.GetMyPendingOrderAsync(userId, showtimeId);

            return Ok(new ApiResponse<PendingOrderDto?>(true, "Lấy thông tin đơn hàng đang chờ thanh toán thành công.", pendingOrder));
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelPendingOrder(Guid orderId)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            await _orderService.CancelPendingOrderAsync(orderId, userId);

            return Ok(new ApiResponse(true, "Hủy đơn hàng thành công."));
        }

        [HttpGet("{orderId}/payment-link")]
        public async Task<IActionResult> GetPaymentLink(Guid orderId)
        {
            var sagaState = await _dbContext.OrderBookingStates
                .FirstOrDefaultAsync(x => x.CorrelationId == orderId);

            if (sagaState == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin thanh toán cho đơn hàng này.");
            }

            if (string.IsNullOrEmpty(sagaState.PaymentLink))
            {
                if (sagaState.CurrentState == "Failed")
                {
                    throw new BusinessRuleException("Đơn hàng đã bị hủy hoặc hết hạn thanh toán.");
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

        [HttpGet("reports/organizer/summary")]
        public async Task<IActionResult> GetOrganizerSummary(CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            var summary = await _reportService.GetOrganizerSummaryAsync(userId, cancellationToken);

            return Ok(new ApiResponse<OrganizerOrderSummaryDto>(true, "Lấy tổng quan đơn hàng của ban tổ chức thành công.", summary));
        }

        [HttpGet("reports/events/{eventId}")]
        public async Task<IActionResult> GetEventReport(Guid eventId, CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            bool isAdminOrMod = User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Admin) ||
                                 User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Moderator);

            var reportData = await _reportService.GetEventReportAsync(eventId, userId, isAdminOrMod, cancellationToken);

            return Ok(new ApiResponse<object>(true, "Lấy báo cáo chi tiết sự kiện thành công.", reportData));
        }

        [HttpGet("reports/events/{eventId}/orders")]
        public async Task<IActionResult> GetEventOrders(
            Guid eventId,
            [FromQuery] GetEventOrdersRequest request,
            CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            bool isAdminOrMod = User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Admin) ||
                                 User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Moderator);

            var result = await _reportService.GetEventOrdersAsync(eventId, userId, isAdminOrMod, request, cancellationToken);

            return Ok(new ApiResponse<object>(true, "Lấy danh sách đơn hàng thành công.", result));
        }

        [HttpGet("reports/events/{eventId}/charts")]
        public async Task<IActionResult> GetEventChartData(
            Guid eventId,
            [FromQuery] string range,
            CancellationToken cancellationToken)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không xác định được danh tính người dùng.");
            }

            bool isAdminOrMod = User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Admin) ||
                                 User.IsInRole(BuildingBlocks.Contracts.Constants.Roles.Moderator);

            var result = await _reportService.GetEventChartDataAsync(eventId, userId, isAdminOrMod, range, cancellationToken);

            return Ok(new ApiResponse<object>(true, "Lấy dữ liệu thống kê sự kiện thành công.", result));
        }

        [Authorize(Roles = BuildingBlocks.Contracts.Constants.Roles.Admin)]
        [HttpGet("reports/admin")]
        public async Task<IActionResult> GetAdminOrders(
            [FromQuery] GetAdminOrdersRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _reportService.GetAdminOrdersAsync(request, cancellationToken);

            return Ok(new ApiResponse<object>(true, "Lấy danh sách đơn hàng thành công.", result));
        }
    }
}
