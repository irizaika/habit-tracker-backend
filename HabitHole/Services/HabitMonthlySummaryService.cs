using HabitHole.Data;
using HabitHole.Models.Dto;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitHole.Services
{
    public class HabitMonthlySummaryService : IHabitMonthlySummaryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDateProvider _dateProvider;

        public HabitMonthlySummaryService(ApplicationDbContext context, IDateProvider dateProvider)
        {
            _context = context;
            _dateProvider = dateProvider;
        }

        public async Task<IEnumerable<HabitMonthlySummaryDto>> GetMonthlySummaryAsync(
            string month,
            bool includeInactive)
        {
            if (!DateOnly.TryParse($"{month}-01", out var firstDay))
                throw new ArgumentException("Invalid month format");

            var now = _dateProvider.Today;

            var start = firstDay;
            var end = firstDay.AddMonths(1).AddDays(-1);

            var daysInMonth = Enumerable
                .Range(0, end.DayNumber - start.DayNumber + 1)
                .Select(offset => start.AddDays(offset))
                .ToList();

            var habitsQuery = _context.Habits.AsQueryable();

            if (!includeInactive)
            {
                habitsQuery = habitsQuery
                    .Where(h => h.ValidFrom <= end &&
                                (h.ValidTo == null || h.ValidTo >= start));
            }

            var habits = await habitsQuery
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.ValidFrom,
                    h.ValidTo,
                    OriginalGoalCount = h.GoalCount,
                    GoalCount = (h.GoalCount > daysInMonth.Count)
                        ? daysInMonth.Count
                        : h.GoalCount,

                    MonthEntries = h.Entries
                        .Where(e => e.Date >= start && e.Date <= end)
                        .Select(e => e.Date),

                    AllEntriesUpToToday = h.Entries
                        .Where(e => e.Date <= now)
                        .Select(e => e.Date)
                })
                .ToListAsync();

            return habits.Select(h =>
            {
                var completedThisMonth = h.MonthEntries
                    .Select(d => d.ToString("yyyy-MM-dd"))
                    .ToHashSet();

                var dailyMap = daysInMonth.ToDictionary(
                    d => d.ToString("yyyy-MM-dd"),
                    d => completedThisMonth.Contains(d.ToString("yyyy-MM-dd"))
                );

                var streak = CalculateCurrentStreak(
                    h.AllEntriesUpToToday,
                    now
                );

                return new HabitMonthlySummaryDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    GoalCount = h.GoalCount,
                    OriginalGoalCount = h.OriginalGoalCount,
                    CompletedCount = completedThisMonth.Count,
                    ProgressPercent = h.GoalCount == 0
                        ? 0
                        : Math.Min(100,
                            (int)((double)completedThisMonth.Count / h.GoalCount * 100)),
                    DailyMap = dailyMap,
                    Streak = streak,
                    ValidFrom = h.ValidFrom.ToString("yyyy-MM-dd"),
                    ValidTo = h.ValidTo?.ToString("yyyy-MM-dd")
                };
            });
        }

        public async Task<int> GetUpdatedStreakAsync(int habitId)
        {
            var now = _dateProvider.Today;

            var entries = await _context.HabitEntries
                .Where(e => e.HabitId == habitId && e.Date <= now)
                .Select(e => e.Date)
                .ToListAsync();

            return CalculateCurrentStreak(entries, now);
        }

        public async Task<List<DailyConsistencyDto>> GetMonthlyConsistency(int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            // Load habits once
            var habits = await _context.Habits
                .Where(h => h.ValidFrom <= end && (h.ValidTo == null || h.ValidTo >= start))
                .ToListAsync();

            var habitIdList = habits.Select(i => i.Id);

            // Load entries once
            var entries = await _context.HabitEntries
                .Where(e => e.Date >= start && e.Date <= end && habitIdList.Contains(e.HabitId))
                .ToListAsync();

            var result = new List<DailyConsistencyDto>();

            for (var day = start; day <= end; day = day.AddDays(1))
            {
                // Active habits that day
                var activeHabitsCount = habits
                    .Count(h => h.ValidFrom <= day && (h.ValidTo == null || h.ValidTo >= day));

                if (activeHabitsCount == 0)
                {
                    result.Add(new DailyConsistencyDto
                    {
                        Date = day.ToString("yyyy-MM-dd"),
                        Value = 0
                    });
                    continue;
                }

                // Completed habits that day
                var completedCount = entries.Count(e => e.Date == day);

                var percentage = (int)Math.Round( (double)completedCount / activeHabitsCount * 100 );

                result.Add(new DailyConsistencyDto
                {
                    Date = day.ToString("yyyy-MM-dd"),
                    Value = percentage
                });
            }

            return result;
        }



        private static int CalculateCurrentStreak(
            IEnumerable<DateOnly> completedDates,
            DateOnly today)
        {
            var set = completedDates.ToHashSet();

            var streak = 0;
            var cursor = today;

            while (set.Contains(cursor) || cursor == today)
            {
                if (cursor != today || set.Contains(cursor))
                    streak++;

                cursor = cursor.AddDays(-1);
            }

            return streak;
        }
    }
}
