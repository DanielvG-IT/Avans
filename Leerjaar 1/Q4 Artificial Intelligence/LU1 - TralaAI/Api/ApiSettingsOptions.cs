using System.ComponentModel.DataAnnotations;

namespace Api;

public class ApiSettingsOptions
{
  public const string SectionName = "ApiSettings";

  [Required(AllowEmptyStrings = false)]
  public string FastApiBaseAddress { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string WeatherApiBaseAddress { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string SensoringApiBaseAddress { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string HolidayApiBaseAddress { get; set; } = string.Empty;
}