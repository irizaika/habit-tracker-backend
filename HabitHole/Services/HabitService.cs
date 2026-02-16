using HabitHole.Data;
using HabitHole.Models;
using HabitHole.Models.Dto;
using HabitHole.Services;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class HabitService : IHabitService
{
    private readonly ApplicationDbContext _context;
    private readonly IDateProvider _dateTime;

    public HabitService(ApplicationDbContext context, IDateProvider dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<IEnumerable<HabitDto>> GetHabitsAsync(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        var habits = await _context.Habits
            .OrderBy(h => h.Name)
            .ToListAsync();

        return habits.Select(h => Mapper.MapToDto(h));
    }

    public async Task<HabitDto> CreateHabitAsync(string name, int goal, DateOnly from, DateOnly? to)
    {
        var habit = new Habit
        {
            Name = name,
            GoalCount = goal,
            GoalPeriodType = GoalPeriodType.MONTH,
            CreatedAt = _dateTime.Today,
            ValidFrom = from,
            ValidTo = to
        };

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();

        return Mapper.MapToDto(habit);
    }

    public async Task UpdateHabitAsync(int id, string name, int goal, DateOnly from, DateOnly? to)
    {
        var habit = await _context.Habits.FindAsync(id);
        if (habit == null)
            throw new KeyNotFoundException("Habit not found");

        habit.Name = name;
        habit.GoalCount = goal;
        habit.ValidFrom = from;
        habit.ValidTo = to;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteHabitAsync(int id)
    {
        var habit = await _context.Habits.FindAsync(id);
        if (habit == null)
            throw new KeyNotFoundException("Habit not found");

        habit.IsDeleted = true;

        var remove = _context.HabitEntries.Where(i => i.HabitId == id);
        _context.HabitEntries.RemoveRange(remove);

        await _context.SaveChangesAsync();
    }
}
