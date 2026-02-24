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
    }
}
