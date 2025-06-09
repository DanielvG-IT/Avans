using System.Text.Json.Serialization;

namespace Api.Models
{
  public class HolidayApiResponse
  {
    [JsonPropertyName("holidays")]
    public required Holiday[] Holidays { get; set; }
  }
}