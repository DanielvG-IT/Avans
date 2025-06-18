using Api.Interfaces;
using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Services
{
  public class DTOService : IDTOService
  {
    // This service can be used to convert LitterType to Category
    // and potentially other DTO-related operations in the future.
    public Category? GetCategory(LitterType? litter)
    {
      if (litter == null)
        return null;

      return litter switch
      {
        LitterType.AluminiumFoil or LitterType.BottleCap or LitterType.Can or LitterType.PopTab => Category.Metal,
        LitterType.Bottle or LitterType.BrokenGlass => Category.Glass,
        LitterType.Carton or LitterType.Paper => Category.Paper,
        LitterType.Cigarette => Category.Organic,
        LitterType.Cup or LitterType.Lid or LitterType.OtherPlastic or LitterType.PlasticBagWrapper or LitterType.PlasticContainer or LitterType.Straw or LitterType.StyrofoamPiece => Category.Plastic,
        _ => Category.Unknown,
      };
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
  }
}