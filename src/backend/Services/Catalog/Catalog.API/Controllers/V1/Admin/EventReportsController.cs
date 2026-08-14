using BuildingBlocks.API.Extensions;
using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.Reports;
using Catalog.Application.Features.Events.Queries.GetAdminEventSummary;
using Catalog.Application.Features.Events.Queries.GetAdminEventsByCategory;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catalog.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/events/reports")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class EventReportsController : ControllerBase
    {
        private readonly ISender _sender;

        public EventReportsController(ISender sender)
        {
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken = default)
        {
            (DateTime from, DateTime to) = ResolveDateRange(dateFrom, dateTo);

            AdminEventSummaryDto result = await _sender.Send(new GetAdminEventSummaryQuery(from, to), cancellationToken);

            return Ok(new ApiResponse<AdminEventSummaryDto>
            {
                Success = true,
                Message = "Lấy thống kê tổng quan sự kiện thành công",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("by-category")]
        public async Task<IActionResult> GetByCategoryAsync(
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            CancellationToken cancellationToken = default)
        {
            (DateTime from, DateTime to) = ResolveDateRange(dateFrom, dateTo);

            List<AdminEventByCategoryDto> result = await _sender.Send(new GetAdminEventsByCategoryQuery(from, to), cancellationToken);

            return Ok(new ApiResponse<List<AdminEventByCategoryDto>>
            {
                Success = true,
                Message = "Lấy thống kê sự kiện theo danh mục thành công",
                Data = result
            });
        }

        private static (DateTime From, DateTime To) ResolveDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime to = (dateTo ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            DateTime from = (dateFrom ?? to.AddDays(-30)).Date;

            return (from, to);
        }
    }
}
