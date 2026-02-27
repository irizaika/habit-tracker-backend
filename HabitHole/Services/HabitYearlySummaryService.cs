using HabitHole.Data;
using HabitHole.Models.Dto;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitHole.Services
{
    public class HabitYearlySummaryService : IHabitYearlySummaryService
    {
        private readonly ApplicationDbContext _context;

        public HabitYearlySummaryService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<List<HabitDailySummaryDto>> GetYearlyCalendar(int year)
        {
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year, 12, 31);

            var habits = _context.Habits.Select(i => i.Id); // filter by user

            var entries = await _context.HabitEntries
                .Where(e => e.Date >= start && e.Date <= end && habits.Contains(e.HabitId))
                .GroupBy(e => e.Date)
                .Select(g => new HabitDailySummaryDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .ToListAsync();

            return entries;
        }


        public async Task<List<HabitYearlySummaryDto>> GetYearlyHabitCalendar(int year)
        {
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year, 12, 31);

            var result = new List<HabitYearlySummaryDto>();

            try
            {
                //{"Translating this query requires the SQL APPLY operation, which is not supported on SQLite."}
                //result = await _context.Habits
                //    .Select(h => new HabitsCalendarDayDto
                //    {
                //        HabitName = h.Name,

                //        Days = h.Entries
                //            .Where(e => e.Date >= start && e.Date <= end)
                //            .GroupBy(e => e.Date)
                //            .Select(g => new CalendarDayDto
                //            {
                //                Day = g.Key.ToString("yyyy-MM-dd"),
                //                Value = g.Count()
                //            })
                //            .ToList()
                //    })
                //    .ToListAsync(); 

                var habits = _context.Habits.Select(i => i.Id); // filter by user

                var flatData = await _context.HabitEntries
                    .Where(e => e.Date >= start && e.Date <= end && habits.Contains(e.HabitId))
                    .Select(e => new
                    {
                        HabitName = e.Habit.Name,
                        e.Date
                    })
                     .ToListAsync();

                result = flatData
                   .GroupBy(x => x.HabitName)
                   .Select(habitGroup => new HabitYearlySummaryDto
                   {
                       HabitName = habitGroup.Key,
                       DailySummaries = habitGroup
                           .GroupBy(x => x.Date)
                           .Select(dayGroup => new HabitDailySummaryDto
                           {
                               Date = dayGroup.Key.ToString("yyyy-MM-dd"),
                               Count = dayGroup.Count()
                           })
                           .ToList()
                   })
                   .ToList();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }


        public async Task<List<HabitMonthlyConsistencyDto>> GetYearlyMonthlyConsistency(int year)
        {
            var startOfYear = new DateOnly(year, 1, 1);
            var endOfYear = new DateOnly(year, 12, 31);

            var habits = await _context.Habits
                .Where(h => h.ValidFrom <= endOfYear &&
                            (h.ValidTo == null || h.ValidTo >= startOfYear))
                .ToListAsync();

            var entries = await _context.HabitEntries
                .Where(e => e.Date.Year == year)
                .ToListAsync();

            var result = new List<HabitMonthlyConsistencyDto>();

            // ALL HABITS COMBINED 
            var allSeries = new HabitMonthlyConsistencyDto
            {
                Name = "All"
            };

            for (int month = 1; month <= 12; month++)
            {
                var monthStart = new DateOnly(year, month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var activeHabits = habits
                    .Where(h => h.ValidFrom <= monthEnd &&
                                (h.ValidTo == null || h.ValidTo >= monthStart))
                    .ToList();

                int totalPossible = 0;
                int totalCompleted = 0;

                for (var day = monthStart; day <= monthEnd; day = day.AddDays(1))
                {
                    var activeThatDay = activeHabits.Count(h =>
                        h.ValidFrom <= day &&
                        (h.ValidTo == null || h.ValidTo >= day));

                    totalPossible += activeThatDay;

                    totalCompleted += entries.Count(e => e.Date == day);
                }

                int percent = totalPossible == 0
                    ? 0
                    : (int)Math.Round((double)totalCompleted / totalPossible * 100);

                allSeries.Data.Add(new MonthlyConsistencyDto
                {
                    Month = monthStart.ToString("MMM"), //month.ToString("00"),
                    Percent = percent
                });
            }

            result.Add(allSeries);

            // PER HABIT 
            foreach (var habit in habits)
            {
                var series = new HabitMonthlyConsistencyDto
                {
                    Name = habit.Name
                };

                for (int month = 1; month <= 12; month++)
                {
                    var monthStart = new DateOnly(year, month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var daysActive = 0;
                    var completed = 0;

                    for (var day = monthStart; day <= monthEnd; day = day.AddDays(1))
                    {
                        if (habit.ValidFrom <= day &&
                            (habit.ValidTo == null || habit.ValidTo >= day))
                        {
                            daysActive++;
                            completed += entries.Count(e =>
                                e.HabitId == habit.Id && e.Date == day);
                        }
                    }

                    int percent = daysActive == 0
                        ? 0
                        : (int)Math.Round((double)completed / daysActive * 100);

                    series.Data.Add(new MonthlyConsistencyDto
                    {
                        Month = monthStart.ToString("MMM"), //month.ToString("00"),
                        Percent = percent
                    });
                }

                result.Add(series);
            }

            return result;
        }



    }
}
