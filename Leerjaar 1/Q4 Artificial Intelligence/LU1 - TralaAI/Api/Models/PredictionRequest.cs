namespace Api.Models
{
    public class PredictionRequest
    {
        public int DayOfWeek { get; set; }
        public int Month { get; set; }
        public bool Holiday { get; set; }
        public int Weather { get; set; }
    }
}