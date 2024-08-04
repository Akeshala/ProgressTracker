using Microsoft.EntityFrameworkCore;
using ProgressTracker.Models;

namespace ProgressTracker.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DailyRecordModel> Subjects { get; set; }
        public DbSet<SessionModel> UserSubjects { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailyRecordModel>()
                .HasIndex(u => new {u.UserId, u.Date})
                .IsUnique();

            modelBuilder.Entity<SessionModel>();
        }
    }
}