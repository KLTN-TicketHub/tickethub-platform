using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Finance.Common.Dtos.Reports;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/finance/reports")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class FinanceReportController : ControllerBase
    {
        private readonly IAdminFinanceReportService _adminFinanceReportService;

        public FinanceReportController(IAdminFinanceReportService adminFinanceReportService)
        {
            _adminFinanceReportService = adminFinanceReportService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken)
        {
            (DateTime from, DateTime to) = ResolveDateRange(dateFrom, dateTo);

            AdminFinanceSummaryDto result = await _adminFinanceReportService.GetSummaryAsync(from, to, cancellationToken);

            return Ok(new ApiResponse<AdminFinanceSummaryDto>(true, "Lấy tổng quan tài chính thành công.", result));
        }

        [HttpGet("trend")]
        public async Task<IActionResult> GetTrendAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken)
        {
            (DateTime from, DateTime to) = ResolveDateRange(dateFrom, dateTo);

            List<AdminFinanceTrendPointDto> result = await _adminFinanceReportService.GetTrendAsync(from, to, cancellationToken);

            return Ok(new ApiResponse<List<AdminFinanceTrendPointDto>>(true, "Lấy dữ liệu xu hướng tài chính thành công.", result));
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken)
        {
            (DateTime from, DateTime to) = ResolveDateRange(dateFrom, dateTo);

            List<AdminFinanceByCategoryDto> result = await _adminFinanceReportService.GetByCategoryAsync(from, to, cancellationToken);

            return Ok(new ApiResponse<List<AdminFinanceByCategoryDto>>(true, "Lấy doanh thu theo danh mục thành công.", result));
        }

        private static (DateTime From, DateTime To) ResolveDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime to = (dateTo ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            DateTime from = (dateFrom ?? to.AddDays(-30)).Date;

            return (from, to);
        }
    }
}
