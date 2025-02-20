namespace CoreLink.WebApi.Models;
/// <summary>
/// Represents a 2D object in the environment.
/// </summary>
public class Object2D
{
    /// <summary>
    /// Gets or sets the unique identifier for the object.
    /// </summary>
    public Guid id;

    /// <summary>
    /// Gets or sets the identifier of the environment the object belongs to.
    /// </summary>
    public required string environmentId;

    /// <summary>
    /// Gets or sets the identifier of the prefab used to create the object.
    /// </summary>
    public required string prefabId;

    /// <summary>
    /// Gets or sets the X position of the object.
    /// </summary>
    public required float positionX;

    /// <summary>
    /// Gets or sets the Y position of the object.
    /// </summary>
    public required float positionY;

    /// <summary>
    /// Gets or sets the X scale of the object.
    /// </summary>
    public required float scaleX;

    /// <summary>
    /// Gets or sets the Y scale of the object.
    /// </summary>
    public required float scaleY;

    /// <summary>
    /// Gets or sets the Z rotation of the object.
    /// </summary>
    public required float rotationZ;

    /// <summary>
    /// Gets or sets the sorting layer of the object.
    /// </summary>
    public required int sortingLayer;
}