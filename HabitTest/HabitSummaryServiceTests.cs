using HabitHole.Data;
using HabitHole.Models;
using HabitHole.Services;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HabitTest
{
    public class HabitSummaryServiceTests
    {
        private readonly string UserId = Guid.NewGuid().ToString();

        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUser = new Mock<ICurrentUserService>();
            mockCurrentUser.Setup(x => x.UserId).Returns(UserId);

            return new ApplicationDbContext(options, mockCurrentUser.Object);
        }

        private static HabitMonthlySummaryService CreateService(ApplicationDbContext context, DateOnly fixedToday)
        {
            var mockDateProvider = new Mock<IDateProvider>();
            mockDateProvider
                .Setup(d => d.Today)
                .Returns(fixedToday);

            var service = new HabitMonthlySummaryService(context, mockDateProvider.Object);

            return service;
        }

        // Streak tests
        [Fact]
        public async Task GetStreak_Should_Return_Correct_Value_When_Today_Is_Set()
        {
            // Arrange
            var context = GetDbContext();

            var habit = new Habit { Name = "Test", GoalCount = 10, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var fixedToday = new DateOnly(2025, 1, 10);

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = fixedToday },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-1) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-2) }
            );

            await context.SaveChangesAsync();

            var service = CreateService(context, fixedToday);

            // Act
            var streak = await service.GetUpdatedStreakAsync(habit.Id);

            // Assert
            Assert.Equal(3, streak);
        }

        [Fact]
        public async Task GetStreak_Should_Return_Correct_Value_When_Today_IsNot_Set()
        {
            // Arrange
            var context = GetDbContext();

            var habit = new Habit { Name = "Test", GoalCount = 10, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var fixedToday = new DateOnly(2025, 1, 10);

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-1) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-2) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-3) }
            );

            await context.SaveChangesAsync();

            var service = CreateService(context, fixedToday);

            // Act
            var streak = await service.GetUpdatedStreakAsync(habit.Id);

            // Assert
            Assert.Equal(3, streak);
        }

        [Fact]
        public async Task GetStreak_Should_Return_Correct_Value_When_TodayAndYesterday_IsNot_Set()
        {
            // Arrange
            var context = GetDbContext();

            var habit = new Habit { Name = "Test", GoalCount = 10, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var fixedToday = new DateOnly(2025, 1, 10);

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-2) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-3) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-4) }
            );

            await context.SaveChangesAsync();

            var service = CreateService(context, fixedToday);

            // Act
            var streak = await service.GetUpdatedStreakAsync(habit.Id);

            // Assert
            Assert.Equal(0, streak);
        }

        // Monthly summary tests
        [Fact]
        public async Task GetMonthlySummaryAsync_InvalidMonth_ShouldThrow()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetMonthlySummaryAsync("invalid-month", false));
        }

        [Fact]
        public async Task Should_Exclude_Inactive_Habits_When_Flag_False()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            var habit = new Habit
            {
                Name = "Old Habit",
                GoalCount = 10,
                ValidFrom = new DateOnly(2024, 1, 1),
                ValidTo = new DateOnly(2024, 1, 31),
                UserId = UserId
            };

            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var result = await service.GetMonthlySummaryAsync("2025-01", false);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Should_Include_Inactive_When_Flag_True()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            var habit = new Habit
            {
                Name = "Old Habit",
                GoalCount = 10,
                ValidFrom = new DateOnly(2024, 1, 1),
                ValidTo = new DateOnly(2024, 1, 31),
                UserId = UserId
            };

            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var result = await service.GetMonthlySummaryAsync("2025-01", true);

            Assert.Single(result);
        }


        [Fact]
        public async Task Should_Calculate_Progress_Correctly()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            var habit = new Habit
            {
                Name = "Reading",
                GoalCount = 10,
                ValidFrom = new DateOnly(2025, 1, 1),
                UserId = UserId
            };

            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 1) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 2) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 3) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 4) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 5) }
            );

            await context.SaveChangesAsync();

            var result = (await service.GetMonthlySummaryAsync("2025-01", false)).First();

            Assert.Equal(5, result.CompletedCount);
            Assert.Equal(50, result.ProgressPercent);
        }

        [Fact]
        public async Task Should_Calculate_Current_Streak()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            var habit = new Habit
            {
                Name = "Workout",
                GoalCount = 10,
                ValidFrom = fixedToday.AddMonths(-1),
                UserId = UserId
            };

            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = fixedToday },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-1) },
                new HabitEntry { HabitId = habit.Id, Date = fixedToday.AddDays(-2) }
            );

            await context.SaveChangesAsync();

            var result = (await service.GetMonthlySummaryAsync(fixedToday.ToString("yyyy-MM"), false)).First();

            Assert.Equal(3, result.Streak);
        }

        [Fact]
        public async Task DailyMap_Should_Have_All_Days_Of_Month()
        {
            var context = GetDbContext();
            var fixedToday = new DateOnly(2025, 1, 10);
            var service = CreateService(context, fixedToday);

            var habit = new Habit
            {
                Name = "Test",
                GoalCount = 5,
                ValidFrom = new DateOnly(2025, 2, 1),
                UserId = UserId
            };

            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var result = (await service.GetMonthlySummaryAsync("2025-02", false)).First();

            Assert.Equal(28, result.DailyMap.Count); // Feb 2025
        }


    }
}