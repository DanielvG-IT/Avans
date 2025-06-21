using System.Text.Json.Serialization;

namespace Api.Services;

public class RetrainRequest
{
  [JsonPropertyName("cameraLocation")]
  public int CameraLocation { get; set; }
}