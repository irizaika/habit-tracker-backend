using HabitHole.Models.Dto;
public interface IHabitService
{
    Task<IEnumerable<HabitDto>> GetHabitsAsync(int year, int month);
    Task<HabitDto> CreateHabitAsync(string name, int goal, DateOnly from, DateOnly? to);
    Task UpdateHabitAsync(int id, string name, int goal, DateOnly from, DateOnly? to);
    Task DeleteHabitAsync(int id);
}
