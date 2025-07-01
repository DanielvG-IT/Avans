namespace Blazor.Models;

public class HeatmapData
{
    public required double TotalLitterPerCamera { get; set; }
    public required decimal Latitude { get; set; }
    public required decimal Longitude { get; set; }
    public required string Location { get; set; }
}
