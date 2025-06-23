namespace Blazor.Models;

public class HeatmapData
{
    public required double totalLitterPerCamera { get; set; }
    public required decimal Latitude { get; set; }
    public required decimal Longitude { get; set; }
    public required string Location { get; set; }
    public required int Id { get; set; }
}

public class HeatMapInput
{
    public required double X { get; set; }
    public required double Y { get; set; }
    public required double Value { get; set; }
}