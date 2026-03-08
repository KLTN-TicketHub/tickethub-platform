using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>, IAggregateRoot
    {
        public string FullName { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime? UpdateAt { get; set; }

        public int? UpdatedBy { get; set; }


        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();


        private readonly List<AuditLog> _auditLogs = new List<AuditLog>();
        public IReadOnlyCollection<AuditLog> AuditLogs => _auditLogs.AsReadOnly();
    }
}
