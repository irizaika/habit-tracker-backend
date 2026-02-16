using HabitHole.Data;
using HabitHole.Models;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class HabitEntryService : IHabitEntryService
{
    private readonly ApplicationDbContext _context;

    public HabitEntryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DateOnly>> GetEntriesAsync(int habitId, string month)
    {
        if (!DateOnly.TryParse($"{month}-01", out var firstDay))
            throw new ArgumentException("Invalid month format (yyyy-MM)");

        var start = firstDay;
        var end = firstDay.AddMonths(1);

        return await _context.HabitEntries
            .Where(e =>
                e.HabitId == habitId &&
                e.Date >= start &&
                e.Date < end)
            .Select(e => e.Date)
            .ToListAsync();
    }

    public async Task AddEntryAsync(int habitId, DateOnly date)
    {
        var habitExists = await _context.Habits.AnyAsync(h => h.Id == habitId);
        if (!habitExists)
            throw new KeyNotFoundException("Habit not found");

        var exists = await _context.HabitEntries.AnyAsync(e =>
            e.HabitId == habitId &&
            e.Date == date);

        if (exists)
            throw new InvalidOperationException("Entry already exists for this date");

        var entry = new HabitEntry
        {
            HabitId = habitId,
            Date = date
        };

        _context.HabitEntries.Add(entry);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEntryAsync(int habitId, DateOnly date)
    {
        var entry = await _context.HabitEntries.FirstOrDefaultAsync(e =>
            e.HabitId == habitId &&
            e.Date == date);

        if (entry == null)
            throw new KeyNotFoundException("Entry not found");

        _context.HabitEntries.Remove(entry);
        await _context.SaveChangesAsync();
    }
}
