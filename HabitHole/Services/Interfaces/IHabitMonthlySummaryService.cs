using HabitHole.Models.Dto;

namespace HabitHole.Services.Interfaces
{
    public interface IHabitMonthlySummaryService
    {
        Task<IEnumerable<HabitMonthlySummaryDto>> GetMonthlySummaryAsync(
            string month, bool includeInactive);

        Task<int> GetUpdatedStreakAsync(int habitId);

        Task<List<DailyConsistencyDto>> GetMonthlyConsistency(int year, int month);
    }
}
