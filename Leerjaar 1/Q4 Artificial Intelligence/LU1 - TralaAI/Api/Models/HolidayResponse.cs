using System.Text.Json.Serialization;

namespace Api.Models;

public class HolidayResponse
{
  [JsonPropertyName("holidays")]
  public required Holiday[] Holidays { get; set; }
}

public class Holiday
{
  [JsonPropertyName("name")]
  public required string Name { get; set; }

  [JsonPropertyName("date")]
  public required string Date { get; set; }

  [JsonPropertyName("public")]
  public bool Public { get; set; }
}