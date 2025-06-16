namespace Api.Models
{
    public class PredictionDto
    {
        public int DayOfWeek { get; set; }    // 0 = Sunday, 1 = Monday, etc.
        public int Month { get; set; }        // 1–12
        public bool Holiday { get; set; }     // true/false
        public int Weather { get; set; }      // e.g. encoded weather category
    }
}