using System.ComponentModel.DataAnnotations;

namespace HabitHole.Models
{
    public class HabitEntry
    {
        public int Id { get; set; }

        [Required]
        public int HabitId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public Habit Habit { get; set; } = null!;
    }
}
