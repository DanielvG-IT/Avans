using System.Text.Json.Serialization;

namespace Api.Models;

public class RetrainRequest
{
  [JsonPropertyName("cameraLocation")]
  public int CameraLocation { get; set; }
}