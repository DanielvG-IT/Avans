using Blazor.Models.Enums;

namespace Blazor.Models;

public class Litter
{
    public int Id { get; set; }
    public LitterCategory? Type { get; set; }
    public DateTime TimeStamp { get; set; }
    public double Confidence { get; set; }
    public WeatherCategory? Weather { get; set; }
    public int Temperature { get; set; }
    public string? Location { get; set; }
    public bool IsHoliday { get; set; }
}