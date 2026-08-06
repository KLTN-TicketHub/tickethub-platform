using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Common.Dtos.Reports;
using Payment.Infrastructure.Interfaces.IServices;

namespace Payment.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/payment/reports")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class PaymentReportController : ControllerBase
    {
        private readonly IAdminPaymentReportService _adminPaymentReportService;

        public PaymentReportController(IAdminPaymentReportService adminPaymentReportService)
        {
            _adminPaymentReportService = adminPaymentReportService;
        }

        [HttpGet("gateway-stats")]
        public async Task<IActionResult> GetGatewayStatsAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken)
        {
            DateTime to = (dateTo ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            DateTime from = (dateFrom ?? to.AddDays(-30)).Date;

            List<AdminPaymentGatewayStatsDto> result = await _adminPaymentReportService.GetGatewayStatsAsync(from, to, cancellationToken);

            return Ok(new ApiResponse<List<AdminPaymentGatewayStatsDto>>(true, "Lấy thống kê cổng thanh toán thành công.", result));
        }
    }
}
