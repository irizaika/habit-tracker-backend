using HabitHole.Data;
using HabitHole.Models;
using HabitHole.Services;
using HabitHole.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HabitTest
{
    public class HabitEntryServiceTests
    {
        private readonly string UserId = Guid.NewGuid().ToString();

        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUser = new Mock<ICurrentUserService>();
            mockCurrentUser.Setup(x => x.UserId).Returns(UserId);

            return new ApplicationDbContext(options, mockCurrentUser.Object);
        }

        private static HabitEntryService CreateService(ApplicationDbContext context)
        {
            return new HabitEntryService(context);
        }

        [Fact]
        public async Task GetEntriesAsync_InvalidMonth_ShouldThrow()
        {
            var context = CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetEntriesAsync(1, "invalid"));
        }

        [Fact]
        public async Task GetEntriesAsync_Should_Return_Only_Month_Entries()
        {
            var context = CreateContext();

            var habit = new Habit { Name = "Test", GoalCount = 5, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            context.HabitEntries.AddRange(
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 5) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 1, 10) },
                new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2025, 2, 1) } // different month
            );

            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetEntriesAsync(habit.Id, "2025-01");

            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(new DateOnly(2025, 2, 1), result);
        }

        [Fact]
        public async Task AddEntryAsync_Should_Add_Entry()
        {
            var context = CreateContext();

            var habit = new Habit { Name = "Test", GoalCount = 5, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var date = new DateOnly(2025, 1, 1);

            await service.AddEntryAsync(habit.Id, date);

            var entry = await context.HabitEntries.FirstAsync();

            Assert.Equal(date, entry.Date);
        }

        [Fact]
        public async Task AddEntryAsync_Should_Throw_If_Duplicate()
        {
            var context = CreateContext();

            var habit = new Habit { Name = "Test", GoalCount = 5, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var date = new DateOnly(2025, 1, 1);

            context.HabitEntries.Add(new HabitEntry
            {
                HabitId = habit.Id,
                Date = date
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddEntryAsync(habit.Id, date));
        }


        [Fact]
        public async Task AddEntryAsync_Should_Throw_If_HabitId_NotExists()
        {
            var context = CreateContext();

            var habit = new Habit { Name = "Test", GoalCount = 5, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var date = new DateOnly(2025, 1, 1);

            context.HabitEntries.Add(new HabitEntry
            {
                HabitId = habit.Id,
                Date = date
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.AddEntryAsync(999, date));
        }

        [Fact]
        public async Task RemoveEntryAsync_Should_Remove_Entry()
        {
            var context = CreateContext();

            var habit = new Habit { Name = "Test", GoalCount = 5, UserId = UserId };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var date = new DateOnly(2025, 1, 1);

            context.HabitEntries.Add(new HabitEntry
            {
                HabitId = habit.Id,
                Date = date
            });

            await context.SaveChangesAsync();

            var service = CreateService(context);

            await service.RemoveEntryAsync(habit.Id, date);

            var entries = await context.HabitEntries.ToListAsync();

            Assert.Empty(entries);
        }

        [Fact]
        public async Task RemoveEntryAsync_Should_Throw_If_NotFound()
        {
            var context = CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.RemoveEntryAsync(1, new DateOnly(2025, 1, 1)));
        }

    }
}
