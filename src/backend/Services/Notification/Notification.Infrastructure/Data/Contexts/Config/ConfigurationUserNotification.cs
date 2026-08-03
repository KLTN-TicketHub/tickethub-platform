using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationUserNotification : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.RecipientUserId)
                .IsRequired(false);

            builder.Property(n => n.TargetRole)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(n => n.LinkUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(n => n.ReferenceId)
                .IsRequired(false);

            builder.Property(n => n.IsRead)
                .IsRequired();

            builder.Property(n => n.ReadAt)
                .IsRequired(false);

            builder.Metadata
                .FindNavigation(nameof(UserNotification.Reads))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(n => new { n.RecipientUserId, n.CreatedAt });

            builder.HasIndex(n => new { n.TargetRole, n.CreatedAt });
        }
    }
}
