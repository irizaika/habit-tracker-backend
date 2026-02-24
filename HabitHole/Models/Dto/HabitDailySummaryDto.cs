namespace HabitHole.Models.Dto
{
    public class HabitDailySummaryDto
    {
        public string Date { get; set; } = default!; // yyyy-MM-dd
        public int Count { get; set; }
    }

}
