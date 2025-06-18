using System.ComponentModel.DataAnnotations;

namespace Api;

public class ApiKeysOptions
{
  public const string SectionName = "ApiKeys";

  [Required(AllowEmptyStrings = false)]
  public string HolidayApiKey { get; internal set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string SensoringApiKey { get; internal set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string WeatherApiKey { get; internal set; } = string.Empty;
}