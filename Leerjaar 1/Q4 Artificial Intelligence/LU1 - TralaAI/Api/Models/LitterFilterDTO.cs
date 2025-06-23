using Api.Models.Enums;

namespace Api.Models
{
    public class LitterFilterDto
    {
        // TODO Add cameraId filter
        public LitterCategory? Type { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? MinTemperature { get; set; }
        public int? MaxTemperature { get; set; }
    }
}