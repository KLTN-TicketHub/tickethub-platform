using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationUserNotificationRead : IEntityTypeConfiguration<UserNotificationRead>
    {
        public void Configure(EntityTypeBuilder<UserNotificationRead> builder)
        {
            builder.ToTable("NotificationReads");

            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Notification)
                .WithMany(n => n.Reads)
                .HasForeignKey(r => r.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.ReadAt)
                .IsRequired();

            builder.HasIndex(r => new { r.NotificationId, r.UserId })
                .IsUnique();
        }
    }
}
