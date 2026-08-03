using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Common.Dtos.Notifications;
using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;

        public NotificationsController(
            INotificationService notificationService,
            ICurrentUserService currentUserService)
        {
            _notificationService = notificationService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsAsync(
            [FromQuery] bool onlyUnread,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken)
        {
            Guid userId = GetCurrentUserId();

            PaginatedResult<NotificationDto> result = await _notificationService.GetInboxAsync(
                userId,
                _currentUserService.Roles,
                onlyUnread,
                pageNumber == 0 ? 1 : pageNumber,
                pageSize == 0 ? 12 : pageSize,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedResult<NotificationDto>>(true, "Lấy danh sách thông báo thành công.", result));
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCountAsync(CancellationToken cancellationToken)
        {
            Guid userId = GetCurrentUserId();

            int result = await _notificationService.GetUnreadCountAsync(
                userId,
                _currentUserService.Roles,
                cancellationToken);

            return Ok(new ApiResponse<int>(true, "Lấy số thông báo chưa đọc thành công.", result));
        }

        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsReadAsync(Guid id, CancellationToken cancellationToken)
        {
            Guid userId = GetCurrentUserId();

            await _notificationService.MarkAsReadAsync(
                id,
                userId,
                _currentUserService.Roles,
                cancellationToken);

            return Ok(new ApiResponse(true, "Đã đánh dấu thông báo là đã đọc."));
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsReadAsync(CancellationToken cancellationToken)
        {
            Guid userId = GetCurrentUserId();

            await _notificationService.MarkAllAsReadAsync(
                userId,
                _currentUserService.Roles,
                cancellationToken);

            return Ok(new ApiResponse(true, "Đã đánh dấu tất cả thông báo là đã đọc."));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteNotificationAsync(Guid id, CancellationToken cancellationToken)
        {
            Guid userId = GetCurrentUserId();

            await _notificationService.DeleteAsync(id, userId, cancellationToken);

            return Ok(new ApiResponse(true, "Xoá thông báo thành công."));
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId
                ?? throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Không thể xác định danh tính người dùng.");
        }
    }
}
