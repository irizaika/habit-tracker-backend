namespace HabitHole.Services.Interfaces
{
    public interface IHabitEntryService
    {
        Task<List<DateOnly>> GetEntriesAsync(int habitId, string month);
        Task AddEntryAsync(int habitId, DateOnly date);
        Task RemoveEntryAsync(int habitId, DateOnly date);
    }
}
