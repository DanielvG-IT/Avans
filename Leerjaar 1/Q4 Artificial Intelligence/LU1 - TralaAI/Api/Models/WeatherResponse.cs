namespace Api.Models;

public class WeatherResponse
{
    public Location Location { get; set; } = new();
    public CurrentWeather Current { get; set; } = new();
    public Forecast Forecast { get; set; } = new();
}

public class Location
{
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string TzId { get; set; } = string.Empty;
    public long LocaltimeEpoch { get; set; }
    public string Localtime { get; set; } = string.Empty;
}

public class CurrentWeather
{
    public long LastUpdatedEpoch { get; set; }
    public string LastUpdated { get; set; } = string.Empty;
    public decimal TempC { get; set; }
    public decimal TempF { get; set; }
    public int IsDay { get; set; }
    public Condition Condition { get; set; } = new();
    public decimal WindMph { get; set; }
    public decimal WindKph { get; set; }
    public int WindDegree { get; set; }
    public string WindDir { get; set; } = string.Empty;
    public decimal PressureMb { get; set; }
    public decimal PressureIn { get; set; }
    public decimal PrecipMm { get; set; }
    public decimal PrecipIn { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    public decimal FeelslikeC { get; set; }
    public decimal FeelslikeF { get; set; }
    public decimal WindchillC { get; set; }
    public decimal WindchillF { get; set; }
    public decimal HeatindexC { get; set; }
    public decimal HeatindexF { get; set; }
    public decimal DewpointC { get; set; }
    public decimal DewpointF { get; set; }
    public decimal VisKm { get; set; }
    public decimal VisMiles { get; set; }
    public decimal Uv { get; set; }
    public decimal GustMph { get; set; }
    public decimal GustKph { get; set; }
}

public class Forecast
{
    public List<ForecastDay> Forecastday { get; set; } = [];
}

public class ForecastDay
{
    public string Date { get; set; } = string.Empty;
    public long DateEpoch { get; set; }
    public Day Day { get; set; } = new();
    public Astro Astro { get; set; } = new();
    public List<Hour> Hour { get; set; } = [];
    public AirQuality AirQuality { get; set; } = new();
}

public class Day
{
    public decimal MaxtempC { get; set; }
    public decimal MaxtempF { get; set; }
    public decimal MintempC { get; set; }
    public decimal MintempF { get; set; }
    public decimal AvgtempC { get; set; }
    public decimal AvgtempF { get; set; }
    public decimal MaxwindMph { get; set; }
    public decimal MaxwindKph { get; set; }
    public decimal TotalprecipMm { get; set; }
    public decimal TotalprecipIn { get; set; }
    public decimal TotalsnowCm { get; set; }
    public decimal AvgvisKm { get; set; }
    public decimal AvgvisMiles { get; set; }
    public int Avghumidity { get; set; }
    public Condition Condition { get; set; } = new();
    public decimal Uv { get; set; }
    public int DailyWillItRain { get; set; }
    public int DailyWillItSnow { get; set; }
    public int DailyChanceOfRain { get; set; }
    public int DailyChanceOfSnow { get; set; }
}

public class Astro
{
    public string Sunrise { get; set; } = string.Empty;
    public string Sunset { get; set; } = string.Empty;
    public string Moonrise { get; set; } = string.Empty;
    public string Moonset { get; set; } = string.Empty;
    public string MoonPhase { get; set; } = string.Empty;
    public decimal MoonIllumination { get; set; }
    public int IsMoonUp { get; set; }
    public int IsSunUp { get; set; }
}

public class Hour
{
    public long TimeEpoch { get; set; }
    public string Time { get; set; } = string.Empty;
    public decimal TempC { get; set; }
    public decimal TempF { get; set; }
    public int IsDay { get; set; }
    public Condition Condition { get; set; } = new();
    public decimal WindMph { get; set; }
    public decimal WindKph { get; set; }
    public int WindDegree { get; set; }
    public string WindDir { get; set; } = string.Empty;
    public decimal PressureMb { get; set; }
    public decimal PressureIn { get; set; }
    public decimal PrecipMm { get; set; }
    public decimal PrecipIn { get; set; }
    public decimal SnowCm { get; set; }
    public int Humidity { get; set; }
    public int Cloud { get; set; }
    public decimal FeelslikeC { get; set; }
    public decimal FeelslikeF { get; set; }
    public decimal WindchillC { get; set; }
    public decimal WindchillF { get; set; }
    public decimal HeatindexC { get; set; }
    public decimal HeatindexF { get; set; }
    public decimal DewpointC { get; set; }
    public decimal DewpointF { get; set; }
    public int WillItRain { get; set; }
    public int WillItSnow { get; set; }
    public int ChanceOfRain { get; set; }
    public int ChanceOfSnow { get; set; }
    public decimal VisKm { get; set; }
    public decimal VisMiles { get; set; }
    public decimal GustMph { get; set; }
    public decimal GustKph { get; set; }
    public decimal Uv { get; set; }
    public decimal ShortRad { get; set; }
    public decimal DiffRad { get; set; }
    public AirQuality AirQuality { get; set; } = new();
}

public class Condition
{
    public string Text { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Code { get; set; }
}

public class AirQuality
{
    // TODO May define based on AQI schema.
}