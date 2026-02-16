using HabitHole.Data;
using HabitHole.Models;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

public class HabitServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private HabitService CreateService(ApplicationDbContext context, DateOnly fixedToday)
    {
        var mockDateProvider = new Mock<IDateProvider>();
        mockDateProvider
            .Setup(d => d.Today)
            .Returns(fixedToday);

        var service = new HabitService(context, mockDateProvider.Object);

        return service;
    }

    // Global query filter tests
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

    // CreateHabitAsync tests

    [Fact]
    public async Task CreateHabitAsync_Should_Add_Habit()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        var from = new DateOnly(2025, 1, 1);

        var result = await service.CreateHabitAsync(
            "Reading", 10, from, null);

        var habitInDb = await context.Habits.FirstAsync();

        Assert.Equal("Reading", habitInDb.Name);
        Assert.Equal(10, habitInDb.GoalCount);
        Assert.False(habitInDb.IsDeleted);
        Assert.Equal("Reading", result.Name);
    }

    // UpdateHabitAsync tests
    [Fact]
    public async Task UpdateHabitAsync_Should_Update_Fields()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        var habit = new Habit
        {
            Name = "Old",
            GoalCount = 5,
            ValidFrom = new DateOnly(2025, 1, 1)
        };

        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        await service.UpdateHabitAsync(
            habit.Id,
            "New",
            20,
            new DateOnly(2025, 2, 1),
            null
        );

        var updated = await context.Habits.FindAsync(habit.Id);

        Assert.Equal("New", updated!.Name);
        Assert.Equal(20, updated.GoalCount);
    }

    [Fact]
    public async Task UpdateHabitAsync_Should_Throw_When_NotFound()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateHabitAsync(
                999,
                "Test",
                5,
                new DateOnly(2025, 1, 1),
                null
            ));
    }

    // DeleteHabitAsync tests
    [Fact]
    public async Task DeleteHabitAsync_Should_Set_IsDeleted_True()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        var habit = new Habit
        {
            Name = "Test",
            GoalCount = 5
        };

        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        await service.DeleteHabitAsync(habit.Id);

        var deleted = await context.Habits.IgnoreQueryFilters()
            .FirstAsync();

        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task DeleteHabitAsync_Should_Remove_Entries()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        var habit = new Habit
        {
            Name = "Test",
            GoalCount = 5
        };

        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        context.HabitEntries.AddRange(
            new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 1) },
            new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 2) }
        );

        await context.SaveChangesAsync();

        await service.DeleteHabitAsync(habit.Id);

        var entries = await context.HabitEntries.ToListAsync();

        Assert.Empty(entries);
    }

    [Fact]
    public async Task DeleteHabitAsync_Should_Throw_When_NotFound()
    {
        var context = GetDbContext();
        var fixedToday = new DateOnly(2025, 1, 10);
        var service = CreateService(context, fixedToday);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.DeleteHabitAsync(999));
    }



}
