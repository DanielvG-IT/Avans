namespace Api.Models
{
    public class FastApiWeatherRequirements
    {
        public required DateTime Date { get; set; }
        public required string Condition { get; set; }
        public required int ConditionCode { get; set; }
        public required double Temperature { get; set; }
    }
}