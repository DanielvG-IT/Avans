namespace Api.Models;

public class PredictionResponse
{
    public required DateTime Date { get; set; }
    public required List<Prediction> Predictions { get; set; }
}

public class Prediction
{
    public required string Type { get; set; }
    public required double Value { get; set; }
}