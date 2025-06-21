
namespace Blazor.Models;

public class Camera
{
  public int Id { get; set; }
  public decimal Latitude { get; set; }
  public decimal Longitude { get; set; }
  public required string Location { get; set; }
}