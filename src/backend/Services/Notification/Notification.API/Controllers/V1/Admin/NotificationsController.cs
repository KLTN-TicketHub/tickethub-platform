using BuildingBlocks.Contracts.Constants;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Common.Dtos.Notifications;
using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.API.Controllers.V1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/notifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = Roles.Admin)]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotificationAsync(
            [FromBody] SendNotificationRequest request,
            CancellationToken cancellationToken)
        {
            NotificationRequestedEvent notificationRequest = BuildRequest(request);

            if (request.ScheduledAt.HasValue)
            {
                ScheduledNotificationDto scheduled = await _notificationService.ScheduleAsync(
                    notificationRequest,
                    request.ScheduledAt.Value,
                    cancellationToken);

                return Ok(new ApiResponse<ScheduledNotificationDto>(true, "Đã hẹn giờ gửi thông báo thành công.", scheduled));
            }

            NotificationDto result = await _notificationService.CreateAsync(notificationRequest, cancellationToken);

            return Ok(new ApiResponse<NotificationDto>(true, "Gửi thông báo thành công.", result));
        }

        [HttpGet]
        public async Task<IActionResult> GetSentNotificationsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            PaginatedResult<SentNotificationDto> result = await _notificationService.GetSentAsync(
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<SentNotificationDto>>(true, "Lấy lịch sử thông báo đã gửi thành công.", result));
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStatsAsync(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken cancellationToken)
        {
            DateTime toUtc = to?.ToUniversalTime() ?? DateTime.UtcNow;
            DateTime fromUtc = from?.ToUniversalTime() ?? toUtc.AddDays(-30);

            NotificationStatsDto result = await _notificationService.GetStatsAsync(fromUtc, toUtc, cancellationToken);

            return Ok(new ApiResponse<NotificationStatsDto>(true, "Lấy thống kê thông báo thành công.", result));
        }

        [HttpGet("{id:guid}/stats")]
        public async Task<IActionResult> GetDetailStatsAsync(Guid id, CancellationToken cancellationToken)
        {
            NotificationDetailStatsDto result = await _notificationService.GetDetailStatsAsync(id, cancellationToken);

            return Ok(new ApiResponse<NotificationDetailStatsDto>(true, "Lấy thống kê chi tiết thông báo thành công.", result));
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledNotificationsAsync(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            PaginatedResult<ScheduledNotificationDto> result = await _notificationService.GetScheduledAsync(
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<ScheduledNotificationDto>>(true, "Lấy danh sách thông báo hẹn giờ thành công.", result));
        }

        [HttpDelete("scheduled/{id:guid}")]
        public async Task<IActionResult> CancelScheduledNotificationAsync(Guid id, CancellationToken cancellationToken)
        {
            await _notificationService.CancelScheduledAsync(id, cancellationToken);

            return Ok(new ApiResponse(true, "Đã huỷ thông báo hẹn giờ."));
        }

        private static NotificationRequestedEvent BuildRequest(SendNotificationRequest request)
        {
            return new NotificationRequestedEvent
            {
                RecipientUserId = request.RecipientUserId,
                TargetRole = request.TargetRole,
                Type = "Announcement",
                Title = request.Title,
                Message = request.Message,
                LinkUrl = request.LinkUrl
            };
        }
    }
}
