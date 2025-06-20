using System.Text.Json.Serialization;

namespace Api.Models;

public class HolidayApiResponse
{
  [JsonPropertyName("status")]
  public int Status { get; set; }

  [JsonPropertyName("warning")]
  public string? Warning { get; set; }

  [JsonPropertyName("requests")]
  public HolidayApiRequests Requests { get; set; } = default!;

  [JsonPropertyName("holidays")]
  public HolidayApiHoliday[] Holidays { get; set; } = default!;
}

public class HolidayApiRequests
{
  [JsonPropertyName("used")]
  public int Used { get; set; }

  [JsonPropertyName("available")]
  public int Available { get; set; }

  [JsonPropertyName("resets")]
  public string Resets { get; set; } = default!;
}

public class HolidayApiHoliday
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = default!;

  [JsonPropertyName("date")]
  public string Date { get; set; } = default!;

  [JsonPropertyName("observed")]
  public string Observed { get; set; } = default!;

  [JsonPropertyName("public")]
  public bool Public { get; set; }

  [JsonPropertyName("country")]
  public string Country { get; set; } = default!;

  [JsonPropertyName("uuid")]
  public string Uuid { get; set; } = default!;

  [JsonPropertyName("weekday")]
  public HolidayApiWeekday Weekday { get; set; } = default!;
}

public class HolidayApiWeekday
{
  [JsonPropertyName("date")]
  public HolidayApiWeekdayDetail Date { get; set; } = default!;

  [JsonPropertyName("observed")]
  public HolidayApiWeekdayDetail Observed { get; set; } = default!;
}

public class HolidayApiWeekdayDetail
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = default!;

  [JsonPropertyName("numeric")]
  public string Numeric { get; set; } = default!;
}