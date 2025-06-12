namespace Blazor.Models
{
    public class Litter //Can be changed if needed
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; }
        public double Confidence { get; set; }
        public string? Weather { get; set; }
        public int Temperature { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsHoliday { get; set; }
    }
}
