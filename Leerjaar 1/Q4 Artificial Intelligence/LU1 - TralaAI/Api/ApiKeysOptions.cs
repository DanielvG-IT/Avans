using System.ComponentModel.DataAnnotations;

namespace Api;

public class ApiKeysOptions
{
  public const string SectionName = "ApiKeys";

  [Required(AllowEmptyStrings = false)]
  public string HolidayApiKey { get; set; } = string.Empty;

  [Required(AllowEmptyStrings = false)]
  public string SensoringApiKey { get; set; } = string.Empty;
}