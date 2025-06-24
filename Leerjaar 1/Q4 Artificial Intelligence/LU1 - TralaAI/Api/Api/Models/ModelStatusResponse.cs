using System;
using System.Text.Json.Serialization;

namespace Api.Models;

public class ModelStatusResponse
{
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("current_rmse")]
    public double CurrentRmse { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime LastUpdated { get; set; }
}