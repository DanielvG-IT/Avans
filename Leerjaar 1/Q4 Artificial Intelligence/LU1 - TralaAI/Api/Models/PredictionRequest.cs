namespace Api.Models;

public class PredictionRequest
{
    public required string ModelIndex { get; set; }
    public required List<Input> Inputs { get; set; }
}

public class Input
{
    public required int DayOfWeek { get; set; }
    public required int Month { get; set; }
    public required bool Holiday { get; set; }
    public required int Weather { get; set; }
    public required int TemperatureCelcius { get; set; }
    public required bool IsWeekend { get; set; }
    public required string Label { get; set; }
}