using Api.Models.Enums;

namespace Api.Models
{
    public class Litter
    {
        public Guid Id { get; set; }
        public Category? Type { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Confidence { get; set; }
        public string? Weather { get; set; } // TODO: Possibly map to WeatherCondition enum
        public int Temperature { get; set; }
        public string? Location { get; set; } // TODO: Possibly map to Location enum
        public bool IsHoliday { get; set; } //! Komt van externe API
    }
}