using System.Text.Json.Serialization;

namespace Blazor.Models;

public class PredictionResponse
{
    [JsonPropertyName("predictions")]
    public List<Dictionary<string, WastePrediction>> Predictions { get; set; } = [];
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