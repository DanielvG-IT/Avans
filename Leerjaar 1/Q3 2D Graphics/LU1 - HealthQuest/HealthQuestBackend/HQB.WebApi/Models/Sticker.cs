namespace HQB.WebApi.Models;

/// <summary>
/// Represents a sticker with an identifier, name, description, and image URL.
/// </summary>
public class Sticker
{
  /// <summary>
  /// Gets or sets the unique identifier for the sticker.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the sticker.
  /// </summary>
  public required string Name { get; set; }
}
