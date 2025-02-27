using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
    [Range(20, 200)]
    public int maxLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum height of the environment.
    /// </summary>
    [Required]
    [Range(10, 100)]
    public int maxHeight { get; set; }

    /// <summary>
    /// Checks if the given position is within the bounds of the environment.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>True if the position is valid, otherwise false.</returns>
    /// TODO MAAK UNIT TEST VOOR DIT 1/3
    public bool IsPositionValid(int x, int y)
    {
        return x >= 0 && x <= maxLength && y >= 0 && y <= maxHeight;
    }
}
