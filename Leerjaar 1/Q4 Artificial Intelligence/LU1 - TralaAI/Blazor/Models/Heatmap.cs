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
    public required double x { get; set; }
    public required double y { get; set; }
    public required double value { get; set; }
}