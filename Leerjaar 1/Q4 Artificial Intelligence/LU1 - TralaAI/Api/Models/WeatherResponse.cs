namespace Api.Models;

public class WeatherResponse
{
    public required Location Location { get; set; }
    public required CurrentWeather Current { get; set; }
    public required Forecast Forecast { get; set; }
}

public class Location
{
    public required string Name { get; set; }
    public required string Region { get; set; }
    public required string Country { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public required string TimeZoneId { get; set; }
    public required string LocalTime { get; set; }
}

public class CurrentWeather
{
    public required string LastUpdated { get; set; }
    public double TempC { get; set; }
    public double TempF { get; set; }
    public bool IsDay { get; set; }
    public required WeatherCondition Condition { get; set; }
    public double WindMph { get; set; }
    public double WindKph { get; set; }
    public int WindDegree { get; set; }
    public required string WindDir { get; set; }
    public double PressureMb { get; set; }
    public double PressureIn { get; set; }
    public double PrecipMm { get; set; }
    public double PrecipIn { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    public double FeelsLikeC { get; set; }
    public double FeelsLikeF { get; set; }
    public double VisibilityKm { get; set; }
    public double VisibilityMiles { get; set; }
    public double UV { get; set; }
    public double GustMph { get; set; }
    public double GustKph { get; set; }
}

public class Forecast
{
    public required List<ForecastDay> ForecastDays { get; set; }
}

public class ForecastDay
{
    public required string Date { get; set; }
    public required DayWeather Day { get; set; }
    public required Astro Astro { get; set; }
    public required List<HourlyWeather> Hour { get; set; }
}

public class DayWeather
{
    public double MaxTempC { get; set; }
    public double MaxTempF { get; set; }
    public double MinTempC { get; set; }
    public double MinTempF { get; set; }
    public double AvgTempC { get; set; }
    public double AvgTempF { get; set; }
    public double MaxWindMph { get; set; }
    public double MaxWindKph { get; set; }
    public double TotalPrecipMm { get; set; }
    public double TotalPrecipIn { get; set; }
    public double AvgVisibilityKm { get; set; }
    public double AvgVisibilityMiles { get; set; }
    public int AvgHumidity { get; set; }
    public required WeatherCondition Condition { get; set; }
    public double UV { get; set; }
}

public class Astro
{
    public required string Sunrise { get; set; }
    public required string Sunset { get; set; }
    public required string Moonrise { get; set; }
    public required string Moonset { get; set; }
    public required string MoonPhase { get; set; }
    public int MoonIllumination { get; set; }
}

public class HourlyWeather
{
    public required string Time { get; set; }
    public double TempC { get; set; }
    public double TempF { get; set; }
    public bool IsDay { get; set; }
    public required WeatherCondition Condition { get; set; }
    public double WindMph { get; set; }
    public double WindKph { get; set; }
    public int WindDegree { get; set; }
    public required string WindDir { get; set; }
    public double PressureMb { get; set; }
    public double PressureIn { get; set; }
    public double PrecipMm { get; set; }
    public double PrecipIn { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    public double FeelsLikeC { get; set; }
    public double FeelsLikeF { get; set; }
    public double VisibilityKm { get; set; }
    public double VisibilityMiles { get; set; }
    public double UV { get; set; }
}

public class WeatherCondition
{
    public required string Text { get; set; }
    public string? Icon { get; set; }
    public int Code { get; set; }
}