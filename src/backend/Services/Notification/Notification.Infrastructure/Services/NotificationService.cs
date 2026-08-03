using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Domain.Exceptions;
using Notification.Common.Dtos.Notifications;
using Notification.Infrastructure.Entities;
using Notification.Infrastructure.Enums;
using Notification.Infrastructure.Interfaces;
using Notification.Infrastructure.Interfaces.IServices;
using System.Linq.Expressions;

namespace Notification.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationPushService _pushService;

        public NotificationService(IUnitOfWork unitOfWork, INotificationPushService pushService)
        {
            _unitOfWork = unitOfWork;
            _pushService = pushService;
        }

        public async Task<NotificationDto> CreateAsync(
            NotificationRequestedEvent request,
            CancellationToken cancellationToken = default)
        {
            UserNotification notification = new UserNotification(
                request.RecipientUserId,
                NormalizeTargetRole(request.RecipientUserId, request.TargetRole),
                ParseType(request.Type),
                request.Title,
                request.Message,
                request.LinkUrl,
                request.ReferenceId);

            _unitOfWork.UserNotificationRepository.AddEntity(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            NotificationDto dto = ToDto(notification, false);

            await _pushService.PushAsync(
                notification.RecipientUserId,
                notification.TargetRole,
                dto,
                cancellationToken);

            return dto;
        }

        public async Task<PaginatedResult<NotificationDto>> GetInboxAsync(
            Guid userId,
            IEnumerable<string> roles,
            bool onlyUnread,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            List<string> roleList = roles.ToList();

            (IEnumerable<NotificationDto> items, int totalCount) = await _unitOfWork.UserNotificationRepository.GetPagedAsync(
                selector: BuildDtoSelector(userId),
                filter: onlyUnread
                    ? BuildUnreadFilter(userId, roleList)
                    : BuildVisibilityFilter(userId, roleList),
                orderBy: q => q.OrderByDescending(n => n.CreatedAt),
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            return new PaginatedResult<NotificationDto>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<int> GetUnreadCountAsync(
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserNotificationRepository.GetCountAsync(
                BuildUnreadFilter(userId, roles.ToList()),
                cancellationToken);
        }

        public async Task MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default)
        {
            List<string> roleList = roles.ToList();

            UserNotification notification = await GetVisibleNotificationAsync(notificationId, userId, roleList, cancellationToken);

            if (notification.RecipientUserId.HasValue)
            {
                notification.MarkAsRead();
                _unitOfWork.UserNotificationRepository.UpdateEntity(notification);
            }
            else
            {
                await AddBroadcastReadAsync(notification.Id, userId, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAllAsReadAsync(
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default)
        {
            List<string> roleList = roles.ToList();

            IEnumerable<UserNotification> unreadPersonal = await _unitOfWork.UserNotificationRepository.GetAllAsync<UserNotification>(
                filter: n => n.RecipientUserId == userId && !n.IsRead,
                cancellation: cancellationToken);

            foreach (UserNotification notification in unreadPersonal)
            {
                notification.MarkAsRead();
                _unitOfWork.UserNotificationRepository.UpdateEntity(notification);
            }

            IEnumerable<Guid> unreadBroadcastIds = await _unitOfWork.UserNotificationRepository.GetAllAsync(
                filter: n => n.RecipientUserId == null
                    && (n.TargetRole == null || roleList.Contains(n.TargetRole!))
                    && !n.Reads.Any(r => r.UserId == userId),
                selector: n => n.Id,
                cancellation: cancellationToken);

            foreach (Guid notificationId in unreadBroadcastIds)
            {
                _unitOfWork.UserNotificationReadRepository.AddEntity(new UserNotificationRead(notificationId, userId));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            UserNotification notification = await _unitOfWork.UserNotificationRepository.GetByIdAsync(notificationId, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy thông báo với ID {notificationId}.");

            if (!notification.RecipientUserId.HasValue)
            {
                throw new BusinessRuleException("Không thể xoá thông báo chung của hệ thống.");
            }

            if (notification.RecipientUserId.Value != userId)
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn không có quyền xoá thông báo này.");
            }

            _unitOfWork.UserNotificationRepository.DeleteEntity(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PaginatedResult<SentNotificationDto>> GetSentAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            (IEnumerable<SentNotificationDto> items, int totalCount) = await _unitOfWork.UserNotificationRepository.GetPagedAsync(
                selector: n => new SentNotificationDto
                {
                    Id = n.Id,
                    RecipientUserId = n.RecipientUserId,
                    TargetRole = n.TargetRole,
                    Type = n.Type.ToString(),
                    Title = n.Title,
                    Message = n.Message,
                    LinkUrl = n.LinkUrl,
                    CreatedAt = n.CreatedAt
                },
                filter: n => n.Type == NotificationType.Announcement,
                orderBy: q => q.OrderByDescending(n => n.CreatedAt),
                pageNumber: pageNumber,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            return new PaginatedResult<SentNotificationDto>(items, totalCount, pageNumber, pageSize);
        }

        private async Task<UserNotification> GetVisibleNotificationAsync(
            Guid notificationId,
            Guid userId,
            List<string> roles,
            CancellationToken cancellationToken)
        {
            UserNotification notification = await _unitOfWork.UserNotificationRepository.GetByIdAsync(notificationId, cancellationToken)
                ?? throw new NotFoundException($"Không tìm thấy thông báo với ID {notificationId}.");

            bool isVisible = notification.RecipientUserId.HasValue
                ? notification.RecipientUserId.Value == userId
                : notification.TargetRole == null || roles.Contains(notification.TargetRole);

            if (!isVisible)
            {
                throw new BuildingBlocks.Domain.Exceptions.UnauthorizedAccessException("Bạn không có quyền truy cập thông báo này.");
            }

            return notification;
        }

        private async Task AddBroadcastReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
        {
            bool isAlreadyRead = await _unitOfWork.UserNotificationReadRepository.GetCountAsync(
                r => r.NotificationId == notificationId && r.UserId == userId,
                cancellationToken) > 0;

            if (isAlreadyRead)
            {
                return;
            }

            _unitOfWork.UserNotificationReadRepository.AddEntity(new UserNotificationRead(notificationId, userId));
        }

        private static Expression<Func<UserNotification, bool>> BuildVisibilityFilter(Guid userId, List<string> roles)
        {
            return n => n.RecipientUserId == userId
                || (n.RecipientUserId == null && (n.TargetRole == null || roles.Contains(n.TargetRole!)));
        }

        private static Expression<Func<UserNotification, bool>> BuildUnreadFilter(Guid userId, List<string> roles)
        {
            return n => (n.RecipientUserId == userId && !n.IsRead)
                || (n.RecipientUserId == null
                    && (n.TargetRole == null || roles.Contains(n.TargetRole!))
                    && !n.Reads.Any(r => r.UserId == userId));
        }

        private static Expression<Func<UserNotification, NotificationDto>> BuildDtoSelector(Guid userId)
        {
            return n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type.ToString(),
                Title = n.Title,
                Message = n.Message,
                LinkUrl = n.LinkUrl,
                ReferenceId = n.ReferenceId,
                IsRead = n.RecipientUserId != null
                    ? n.IsRead
                    : n.Reads.Any(r => r.UserId == userId),
                CreatedAt = n.CreatedAt
            };
        }

        private static NotificationDto ToDto(UserNotification notification, bool isRead)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Type = notification.Type.ToString(),
                Title = notification.Title,
                Message = notification.Message,
                LinkUrl = notification.LinkUrl,
                ReferenceId = notification.ReferenceId,
                IsRead = isRead,
                CreatedAt = notification.CreatedAt
            };
        }

        private static NotificationType ParseType(string type)
        {
            return Enum.TryParse(type, ignoreCase: true, out NotificationType parsed)
                ? parsed
                : NotificationType.General;
        }

        private static string? NormalizeTargetRole(Guid? recipientUserId, string? targetRole)
        {
            return recipientUserId.HasValue ? null : targetRole;
        }
    }
}
