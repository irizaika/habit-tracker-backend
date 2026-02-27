using HabitHole.Models.Dto;

namespace HabitHole.Services.Interfaces
{
    public interface IHabitYearlySummaryService
    {
        Task<List<HabitDailySummaryDto>> GetYearlyCalendar(int year);
        Task<List<HabitYearlySummaryDto>> GetYearlyHabitCalendar(int year);
        Task<List<HabitMonthlyConsistencyDto>> GetYearlyMonthlyConsistency(int year);
    }
}
