using HabbitHole.Data;
using HabitHole.Models;
using Microsoft.EntityFrameworkCore;

public class HabitServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Deleted_Habit_Should_Not_Be_Returned()
    {
        var context = GetDbContext();

        var habit = new Habit
        {
            Name = "Deleted",
            GoalCount = 5,
            IsDeleted = true
        };

        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        var habits = await context.Habits.ToListAsync();

        Assert.Empty(habits); // because of global query filter
    }

    [Fact]
    public async Task Correct_Number_Of_Habits_Should_Not_Be_Returned()
    {
        var context = GetDbContext();

        var habit = new Habit[]
            { 
                new Habit
                {
                    Name = "Deleted",
                    GoalCount = 5,
                    IsDeleted = true
                },
                new Habit
                {
                    Name = "Active",
                    GoalCount = 5,
                    IsDeleted = false
                }
            };

        context.Habits.AddRange(habit);
        await context.SaveChangesAsync();

        var count = await context.Habits.CountAsync();

        count.Equals(1); // because of global query filter
    }
}
