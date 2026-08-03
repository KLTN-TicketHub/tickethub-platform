using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationScheduledNotification : IEntityTypeConfiguration<ScheduledNotification>
    {
        public void Configure(EntityTypeBuilder<ScheduledNotification> builder)
        {
            builder.ToTable("ScheduledNotifications");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.RecipientUserId)
                .IsRequired(false);

            builder.Property(s => s.TargetRole)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(s => s.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.LinkUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(s => s.ScheduledAt)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(s => s.SentAt)
                .IsRequired(false);

            builder.Property(s => s.CreatedNotificationId)
                .IsRequired(false);

            builder.HasIndex(s => new { s.Status, s.ScheduledAt });
        }
    }
}
