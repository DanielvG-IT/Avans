using Blazor.Models.Enums;

namespace Blazor.Models;

public class Litter
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public double? Confidence { get; set; }
    public string Weather { get; set; } = string.Empty;
    public int Temperature { get; set; }
    public bool IsHoliday { get; set; }
    public Camera Camera { get; set; } = null!;
    public int CameraId { get; set; }

    // Enum helpers (not mapped)
    public LitterCategory LitterCategory
    {
        get
        {
            if (Enum.TryParse<LitterCategory>(Type, true, out var category))
                return category;
            return LitterCategory.Unknown;
        }
        set => Type = value.ToString().ToLowerInvariant();
    }

    public WeatherCategory WeatherCategory
    {
        get
        {
            if (Enum.TryParse<WeatherCategory>(Weather, true, out var category))
                return category;
            return WeatherCategory.Unknown;
        }
        set => Weather = value.ToString().ToLowerInvariant();
    }
}