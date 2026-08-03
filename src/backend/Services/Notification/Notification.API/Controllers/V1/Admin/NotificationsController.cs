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
            NotificationRequestedEvent notificationRequest = new NotificationRequestedEvent
            {
                RecipientUserId = request.RecipientUserId,
                TargetRole = request.TargetRole,
                Type = "Announcement",
                Title = request.Title,
                Message = request.Message,
                LinkUrl = request.LinkUrl
            };

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
    }
}
