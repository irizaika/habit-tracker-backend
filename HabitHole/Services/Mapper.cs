using HabitHole.Models;
using HabitHole.Models.Dto;

namespace HabitHole.Services
{
    public class Mapper
    {
        public static HabitDto MapToDto(Habit h)
        {
            return new HabitDto
            {
                Id = h.Id,
                Name = h.Name,
                CreatedAt = h.CreatedAt,
                GoalCount = h.GoalCount,
                GoalPeriodType = (int)h.GoalPeriodType,
                ValidFrom = h.ValidFrom,
                ValidTo = h.ValidTo
            };
        }
    }
}
