using Api.Models.Enums.DTO;

namespace Api.Models
{

    public class AggregatedTrashDto
    {
        public List<TrashDTO> Litters { get; set; } = [];
    }

    public class TrashDTO
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; }
        public double Confidence { get; set; }
        public string? Weather { get; set; }
        public float Temperature { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

}