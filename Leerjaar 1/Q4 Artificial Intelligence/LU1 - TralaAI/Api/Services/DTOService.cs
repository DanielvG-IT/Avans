using Api.Interfaces;
using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Services;

public class DTOService : IDTOService
{
  // This service can be used to convert LitterType to Category
  // and potentially other DTO-related operations in the future.
  public LitterCategory? GetCategory(LitterType? litter)
  {
    if (litter == null)
      return null;

    return litter switch
    {
      LitterType.AluminiumFoil or LitterType.BottleCap or LitterType.Can or LitterType.PopTab => LitterCategory.Metal,
      LitterType.Bottle or LitterType.BrokenGlass => LitterCategory.Glass,
      LitterType.Carton or LitterType.Paper => LitterCategory.Paper,
      LitterType.Cigarette => LitterCategory.Organic,
      LitterType.Cup or LitterType.Lid or LitterType.OtherPlastic or LitterType.PlasticBagWrapper or LitterType.PlasticContainer or LitterType.Straw or LitterType.StyrofoamPiece => LitterCategory.Plastic,
      _ => LitterCategory.Unknown,
    };
  }

  public LitterCategory? GetCategory(string? litter)
  {
    if (string.IsNullOrWhiteSpace(litter))
      return null;

    // Try to parse as enum first
    if (Enum.TryParse<LitterType>(litter, true, out var parsedLitter))
    {
      return GetCategory(parsedLitter);
    }

    // Handle common descriptive phrases
    var normalized = litter.Trim().ToLowerInvariant();

    if (normalized.Contains("metal") || normalized.Contains("foil") || normalized.Contains("can") || normalized.Contains("cap") || normalized.Contains("tab"))
      return LitterCategory.Metal;
    if (normalized.Contains("glass") || normalized.Contains("bottle"))
      return LitterCategory.Glass;
    if (normalized.Contains("paper") || normalized.Contains("carton"))
      return LitterCategory.Paper;
    if (normalized.Contains("cigarette") || normalized.Contains("organic"))
      return LitterCategory.Organic;
    if (normalized.Contains("plastic") || normalized.Contains("cup") || normalized.Contains("lid") || normalized.Contains("straw") || normalized.Contains("styrofoam") || normalized.Contains("wrapper") || normalized.Contains("container"))
      return LitterCategory.Plastic;

    return LitterCategory.Plastic;
  }

  public WeatherCategory? GetWeatherCategory(WeatherCondition? weather)
  {
    if (weather == null)
      return null;

    return weather switch
    {
      WeatherCondition.Blizzard or WeatherCondition.BlowingSnow or WeatherCondition.PatchySnowNearby or WeatherCondition.PatchySleetNearby or WeatherCondition.PatchyModerateSnow or WeatherCondition.ModerateSnow or WeatherCondition.PatchyHeavySnow or WeatherCondition.HeavySnow or WeatherCondition.LightSnow or WeatherCondition.PatchyLightSnow or WeatherCondition.ModerateOrHeavySnowShowers or WeatherCondition.LightSnowShowers => WeatherCategory.Snowy,
      WeatherCondition.ThunderyOutbreaksInNearby or WeatherCondition.PatchyLightRainInAreaWithThunder or WeatherCondition.ModerateOrHeavyRainInAreaWithThunder or WeatherCondition.PatchyLightSnowInAreaWithThunder or WeatherCondition.ModerateOrHeavySnowInAreaWithThunder => WeatherCategory.Stormy,
      WeatherCondition.PatchyLightRain or WeatherCondition.LightRain or WeatherCondition.ModerateRainAtTimes or WeatherCondition.ModerateRain or WeatherCondition.HeavyRainAtTimes or WeatherCondition.HeavyRain or WeatherCondition.LightRainShower or WeatherCondition.ModerateOrHeavyRainShower or WeatherCondition.TorrentialRainShower => WeatherCategory.Rainy,
      WeatherCondition.Mist or WeatherCondition.FreezingFog or WeatherCondition.Fog or WeatherCondition.PatchyFreezingDrizzleNearby => WeatherCategory.Misty,
      WeatherCondition.Cloudy or WeatherCondition.Overcast or WeatherCondition.PartlyCloudy => WeatherCategory.Cloudy,
      WeatherCondition.Sunny => WeatherCategory.Sunny,
      _ => WeatherCategory.Unknown,
    };
  }

  public WeatherCategory? GetWeatherCategory(string? weather)
  {
    if (string.IsNullOrWhiteSpace(weather))
      return null;

    // Try to parse as enum first
    if (Enum.TryParse<WeatherCondition>(weather, true, out var parsedWeather))
    {
      return GetWeatherCategory(parsedWeather);
    }

    // Handle common descriptive phrases
    var normalized = weather.Trim().ToLowerInvariant();

    // Map common phrases to WeatherCondition
    if (normalized.Contains("rain"))
      return WeatherCategory.Rainy;
    if (normalized.Contains("snow"))
      return WeatherCategory.Snowy;
    if (normalized.Contains("storm") || normalized.Contains("thunder"))
      return WeatherCategory.Stormy;
    if (normalized.Contains("mist") || normalized.Contains("fog"))
      return WeatherCategory.Misty;
    if (normalized.Contains("cloud"))
      return WeatherCategory.Cloudy;
    if (normalized.Contains("sun") || normalized.Contains("clear"))
      return WeatherCategory.Sunny;

    return WeatherCategory.Unknown;
  }

  public int? GetWeatherCategoryIndex(WeatherCategory? weatherCategory)
  {
    if (weatherCategory is null)
      return null;

    return weatherCategory switch
    {
      WeatherCategory.Snowy => 1,
      WeatherCategory.Stormy => 2,
      WeatherCategory.Rainy => 3,
      WeatherCategory.Misty => 4,
      WeatherCategory.Cloudy => 5,
      WeatherCategory.Sunny => 6,
      _ => null,
    };
  }
}