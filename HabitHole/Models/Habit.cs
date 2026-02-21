using System.ComponentModel.DataAnnotations;

namespace HabitHole.Models
{
    public class Habit
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Range(1, 1000)]
        public int GoalCount { get; set; } // e.g. 20 times per month
        public GoalPeriodType GoalPeriodType { get; set; } = GoalPeriodType.MONTH;
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly ValidFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly? ValidTo { get; set; } = null;
        public bool IsDeleted { get; set; } = false;

        public string UserId { get; set; } = null!;

        public ICollection<HabitEntry> Entries { get; set; } = [];
    }
}