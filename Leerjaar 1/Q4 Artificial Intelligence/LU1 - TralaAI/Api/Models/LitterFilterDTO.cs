namespace Api.Models
{
    public class LitterFilterDto
    {
        public string? Type { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? MinTemperature { get; set; }
        public int? MaxTemperature { get; set; }
    }
}