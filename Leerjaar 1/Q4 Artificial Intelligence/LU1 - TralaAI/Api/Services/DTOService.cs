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

    if (Enum.TryParse<LitterType>(litter, true, out var parsedLitter))
    {
      return GetCategory(parsedLitter);
    }

    return LitterCategory.Unknown;
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

    if (Enum.TryParse<WeatherCondition>(weather, true, out var parsedWeather))
    {
      return GetWeatherCategory(parsedWeather);
    }

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

  public int? GetWeatherCategoryIndex(string? weatherCategory)
  {
    if (string.IsNullOrWhiteSpace(weatherCategory))
      return null;

    if (Enum.TryParse<WeatherCategory>(weatherCategory, true, out var parsedWeatherCategory))
    {
      return GetWeatherCategoryIndex(parsedWeatherCategory);
    }

    return null;
  }
}