namespace HabitHole.Models.Dto
{
    public class HabitYearlySummaryDto
    {
        public string HabitName { get; set; } = String.Empty;
        public List<HabitDailySummaryDto> DailySummaries { get; set; } = [];
    }

}
