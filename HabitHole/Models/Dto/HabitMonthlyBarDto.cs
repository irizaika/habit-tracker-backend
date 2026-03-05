namespace HabitHole.Models.Dto
{
    public class HabitMonthlyBarDto
    {
        public string Month { get; set; } = string.Empty;
        public string HabitName { get; set; } = string.Empty;
        public int Completed { get; set; }
        public int Goal { get; set; }
    }
}
