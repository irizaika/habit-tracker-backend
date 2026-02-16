using HabitHole.Services.Interfaces;

namespace HabitHole.Services
{
    public class DateProvider : IDateProvider
    {
        public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
