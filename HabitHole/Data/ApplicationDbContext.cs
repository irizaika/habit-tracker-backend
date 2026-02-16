using HabitHole.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitHole.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitEntry> HabitEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habit>()
                .HasQueryFilter(h => !h.IsDeleted);

            modelBuilder.Entity<HabitEntry>()
                .HasIndex(e => new { e.HabitId, e.Date })
                .IsUnique();

            modelBuilder.Entity<HabitEntry>()
                .HasOne(e => e.Habit)
                .WithMany(h => h.Entries)
                .HasForeignKey(e => e.HabitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Habit>()
                .Property(h => h.GoalPeriodType)
                .HasConversion<int>();

            modelBuilder.Entity<HabitEntry>()
                .HasQueryFilter(e => !e.Habit.IsDeleted);

        }
    }

}
