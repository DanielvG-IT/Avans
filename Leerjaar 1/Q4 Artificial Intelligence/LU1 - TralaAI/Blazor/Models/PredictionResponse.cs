using System.Text.Json.Serialization;

namespace Blazor.Models;

public class PredictionResponse
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("predictions")]
    public required PredictionValues Predictions { get; set; }
}

public class PredictionValues
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
