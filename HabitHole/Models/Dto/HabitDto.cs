namespace HabitHole.Models.Dto
{
    public class HabitDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GoalCount { get; set; }
        public int GoalPeriodType { get; set; }
        public DateOnly CreatedAt { get; set; }
        public DateOnly ValidFrom { get; set; }
        public DateOnly? ValidTo { get; set; }
        public int ActualCount { get; set; }
    }
}
