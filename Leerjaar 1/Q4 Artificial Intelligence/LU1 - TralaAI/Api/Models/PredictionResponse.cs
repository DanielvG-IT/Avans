using System.Text.Json.Serialization;

namespace Api.Models;

public class PredictionResponse
{
    [JsonPropertyName("predictions")]
    public List<WastePrediction> Predictions { get; set; } = new();
}

public class WastePrediction
{
    [JsonPropertyName("plastic")]
    public double Plastic { get; set; }

    [JsonPropertyName("paper")]
    public double Paper { get; set; }

    [JsonPropertyName("metal")]
    public double Metal { get; set; }

    [JsonPropertyName("glass")]
    public double Glass { get; set; }

    [JsonPropertyName("organic")]
    public double Organic { get; set; }
}