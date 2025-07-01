using System.Text.Json.Serialization;

namespace Api.Models;

public class PredictionRequest
{
    [JsonPropertyName("cameraId")]
    public required int CameraId { get; set; }

    [JsonPropertyName("inputs")]
    public required List<Input> Inputs { get; set; }
}

public class Input
{
    [JsonPropertyName("day_of_week")]
    public required int DayOfWeek { get; set; }

    [JsonPropertyName("month")]
    public required int Month { get; set; }

    [JsonPropertyName("holiday")]
    public required bool Holiday { get; set; }

    [JsonPropertyName("weather")]
    public required int Weather { get; set; }

    [JsonPropertyName("temperature_celcius")]
    public required int TemperatureCelcius { get; set; }

    [JsonPropertyName("is_weekend")]
    public required bool IsWeekend { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }
}