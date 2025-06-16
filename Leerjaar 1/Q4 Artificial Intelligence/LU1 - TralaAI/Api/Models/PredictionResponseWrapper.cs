namespace Api.Models
{
    public class PredictionResponseWrapper
    {
        public List<List<float>> Predictions { get; set; } = new();
    }
}