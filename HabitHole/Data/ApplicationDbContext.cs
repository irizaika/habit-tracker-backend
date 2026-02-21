using HabitHole.Models;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HabitHole.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
             ICurrentUserService currentUserService): base(options) 
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Habit> Habits { get; set; }
        public DbSet<HabitEntry> HabitEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habit>()
                .HasQueryFilter(h =>
                    h.UserId == _currentUserService.UserId &&
                    !h.IsDeleted);

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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Habit>()
                .Where(e => e.State == EntityState.Added))
            {
                entry.Entity.UserId = _currentUserService.UserId!;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

}
