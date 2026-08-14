using AI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationRecommendationResult : IEntityTypeConfiguration<RecommendationResult>
    {
        public void Configure(EntityTypeBuilder<RecommendationResult> builder)
        {
            builder.ToTable("RecommendationResult");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.EventId).IsRequired();
            builder.Property(x => x.Score).IsRequired();
            builder.Property(x => x.Rank).IsRequired();
            builder.Property(x => x.GeneratedAt).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.Rank });
        }
    }
}
