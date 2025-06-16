using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Models
{
    public class Litter
    {
        public Guid Id { get; set; }
        public Category? Type { get; set; }
        public DateTime Date { get; set; }
        public double Confidence { get; set; }
        public WeatherCondition? Weather { get; set; }
        public int Temperature { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsHoliday { get; set; } //! Komt van externe API
    }
}