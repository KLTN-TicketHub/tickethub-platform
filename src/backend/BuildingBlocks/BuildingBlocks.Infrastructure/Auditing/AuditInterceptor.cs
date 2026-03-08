using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.DDD;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Auditing
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditInterceptor> _logger;

        public AuditInterceptor(
            ICurrentUserService currentUser,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditInterceptor> logger)
        {
            _currentUser = currentUser;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                ProcessEntries(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ProcessEntries(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted))
                .ToList();

            var auditLogs = new List<AuditLog>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added when entry.Entity is BaseEntity added:
                        HandleAdded(added, auditLogs);
                        break;

                    case EntityState.Modified when entry.Entity is BaseEntity modified:
                        HandleModified(modified, auditLogs);
                        break;

                    case EntityState.Deleted when entry.Entity is SoftDeleteEntity softDeleted:
                        entry.State = EntityState.Modified;
                        HandleSoftDeleted(softDeleted, auditLogs);
                        break;

                    case EntityState.Deleted when entry.Entity is BaseEntity hardDeleted:
                        HandleHardDeleted(hardDeleted, auditLogs);
                        break;
                }
            }

            if (auditLogs.Count > 0)
                context.Set<AuditLog>().AddRange(auditLogs);
        }

        private void HandleAdded(BaseEntity entity, List<AuditLog> auditLogs)
        {
            entity.SetCreated(_currentUser.UserId);
            _logger.LogInformation("User {User} created entity {EntityType} with Id {EntityId}",
                _currentUser.UserName, entity.GetType().Name, entity.Id);
            auditLogs.Add(BuildAuditLog("Created", entity));
        }

        private void HandleModified(BaseEntity entity, List<AuditLog> auditLogs)
        {
            entity.SetUpdated(_currentUser.UserId);
            _logger.LogInformation("User {User} updated entity {EntityType} with Id {EntityId}",
                _currentUser.UserName, entity.GetType().Name, entity.Id);
            auditLogs.Add(BuildAuditLog("Updated", entity));
        }

        private void HandleSoftDeleted(SoftDeleteEntity entity, List<AuditLog> auditLogs)
        {
            entity.SetDeleted(_currentUser.UserId);
            _logger.LogWarning("User {User} soft-deleted entity {EntityType} with Id {EntityId}",
                _currentUser.UserName, entity.GetType().Name, entity.Id);
            auditLogs.Add(BuildAuditLog("SoftDeleted", entity));
        }

        private void HandleHardDeleted(BaseEntity entity, List<AuditLog> auditLogs)
        {
            _logger.LogWarning("User {User} hard-deleted entity {EntityType} with Id {EntityId}",
                _currentUser.UserName, entity.GetType().Name, entity.Id);
            auditLogs.Add(BuildAuditLog("HardDeleted", entity));
        }

        private AuditLog BuildAuditLog(string action, BaseEntity entity)
        {
            return new AuditLog
            {
                UserId = _currentUser.UserId,
                UserName = _currentUser.UserName,
                Action = action,
                EntityType = entity.GetType().Name,
                EntityId = entity.Id,
                IPAddress = _currentUser.IpAddress,
                UserAgent = _currentUser.DeviceInfo,
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier,
                Source = "EFCore",
                RequestPath = _httpContextAccessor.HttpContext?.Request.Path,
            };
        }
    }
}
