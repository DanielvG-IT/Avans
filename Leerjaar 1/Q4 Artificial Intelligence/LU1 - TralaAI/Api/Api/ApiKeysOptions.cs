using System.ComponentModel.DataAnnotations;

namespace Api;

public class ApiKeysOptions
{
  public const string SectionName = "ApiKeys";

  [Required(AllowEmptyStrings = false)]
  public string SensoringApiKey { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string WeatherApiKey { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string FastApiKey { get; set; } = string.Empty;
}