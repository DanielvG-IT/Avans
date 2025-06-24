using System.Text.Json.Serialization;

namespace Api.Models;

public class HolidayApiResponse
{
  [JsonPropertyName("date")]
  public string Date { get; set; } = default!;

  [JsonPropertyName("localName")]
  public string LocalName { get; set; } = default!;

  [JsonPropertyName("name")]
  public string Name { get; set; } = default!;

  [JsonPropertyName("countryCode")]
  public string CountryCode { get; set; } = default!;

  [JsonPropertyName("fixed")]
  public bool Fixed { get; set; }

  [JsonPropertyName("global")]
  public bool Global { get; set; }

  [JsonPropertyName("counties")]
  public string[]? Counties { get; set; }

  [JsonPropertyName("launchYear")]
  public int? LaunchYear { get; set; }

  [JsonPropertyName("types")]
  public string[] Types { get; set; } = default!;
}
