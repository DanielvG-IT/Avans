namespace HQB.WebApi.Models;

/// <summary>
/// Represents a sticker with an identifier, name, description, and image URL.
/// </summary>
public class Sticker
{
  /// <summary>
  /// Gets or sets the unique identifier for the sticker.
  /// </summary>
  public Guid Id { get; set; } // Changed to Guid

  /// <summary>
  /// Gets or sets the name of the sticker.
  /// </summary>
  public required string Name { get; set; } // nvarchar(50)

  /// <summary>
  /// Gets or sets the description of the sticker.
  /// </summary>
  public required string Description { get; set; } // nvarchar(256)

  /// <summary>
  /// Gets or sets the URL of the sticker's image.
  /// </summary>
  public required string ImageUrl { get; set; } // nvarchar(256)
}
