using System.Text.Json.Serialization;

namespace Api.Models
{
  public class Holiday
  {
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("date")]
    public required string Date { get; set; }

    [JsonPropertyName("public")]
    public bool Public { get; set; }
  }
}