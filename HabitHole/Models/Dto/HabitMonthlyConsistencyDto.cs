namespace HabitHole.Models.Dto
{
    public class HabitMonthlyConsistencyDto
    {
        public string Name { get; set; } = string.Empty; // habit name OR "All"
        public List<MonthlyConsistencyDto> Data { get; set; } = new();
    }
}
