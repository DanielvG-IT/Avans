namespace Api.Models
{
    public class Litter
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; }
        public double Confidence { get; set; }
        public string? Weather { get; set; }
        public int Temperature { get; set; }
        public decimal Latitude { get; set; } //! Staat niet in Sensoring API - Y-as
        public decimal Longitude { get; set; } //! Staat niet in Sensoring API - X-as
        public bool IsHoliday { get; set; } //! Komt van externe API

    }

}