using System.ComponentModel.DataAnnotations;

namespace CoreLink.WebApi.Models;

/// <summary>
/// Represents a 2D environment with specific dimensions and a unique identifier.
/// </summary>
public class Environment2D
{
    /// <summary>
    /// Gets or sets the unique identifier for the environment.
    /// </summary>
    public Guid id { get; set; }

    /// <summary>
    /// Gets or sets the name of the environment.
    /// </summary>
    [Required]
    public string? name { get; set; }

    /// <summary>
    ///   Gets or sets the user identifier.
    /// </summary>
    public string? ownerUserId { get; set; }

    /// <summary>
    /// Gets or sets the maximum length of the environment.
    /// </summary>
    [Required]
    public int maxLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the environment.
    /// </summary>
    [Required]
    public int maxHeight { get; set; }
}
