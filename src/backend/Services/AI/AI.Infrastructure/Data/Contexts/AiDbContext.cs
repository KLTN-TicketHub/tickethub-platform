using AI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.Infrastructure.Data.Contexts
{
    public class AiDbContext : DbContext
    {
        public AiDbContext(DbContextOptions<AiDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<UserEventInteraction> UserEventInteractions { get; set; }
        public DbSet<RecommendationResult> RecommendationResults { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiDbContext).Assembly);
        }
    }
}
