namespace HabitHole.Models.Dto
{
    public class HabitMonthlySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GoalCount { get; set; }
        public int OriginalGoalCount { get; set; }
        public int CompletedCount { get; set; }
        public int ProgressPercent { get; set; }
        public Dictionary<string, bool> DailyMap { get; set; }
        public int Streak { get; set; }

        public string ValidFrom { get; set; }
        public string? ValidTo { get; set; }
    }
}
