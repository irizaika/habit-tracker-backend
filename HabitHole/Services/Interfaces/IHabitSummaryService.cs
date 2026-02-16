using HabitHole.Models.Dto;

public interface IHabitSummaryService
{
    Task<IEnumerable<HabitMonthlySummaryDto>> GetMonthlySummaryAsync(
        string month,
        bool includeInactive);

    Task<int> GetUpdatedStreakAsync(int habitId);
}
