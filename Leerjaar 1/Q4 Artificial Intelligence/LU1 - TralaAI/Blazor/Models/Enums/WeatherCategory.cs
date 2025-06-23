namespace Blazor.Models.Enums
{
    public enum WeatherCategory
    {
        Snowy = 1,
        Stormy = 2,
        Rainy = 3,
        Misty = 4,
        Cloudy = 5,
        Sunny = 6,
        Unknown = 0
    }

    public static partial class CategoryExtensions
    {
        public static string ToFriendlyString(this WeatherCategory category)
        {
            return category switch
            {
                WeatherCategory.Snowy => "snowy",
                WeatherCategory.Stormy => "stormy",
                WeatherCategory.Rainy => "rainy",
                WeatherCategory.Misty => "misty",
                WeatherCategory.Cloudy => "cloudy",
                WeatherCategory.Sunny => "sunny",
                WeatherCategory.Unknown => "unknown",
                _ => "unknown"
            };
        }
    }

}