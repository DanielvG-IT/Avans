using System;

/// <summary>
/// Represents a sticker with an identifier, name, description, and image URL.
/// </summary>
[Serializable]
public class Sticker
{
    /// <summary>
    /// Gets or sets the unique identifier for the sticker.
    /// </summary>
    public string id;

    /// <summary>
    /// Gets or sets the name of the sticker.
    /// </summary>
    public string name;

    /// <summary>
    /// Gets or sets the date when the sticker got unlocked.
    /// </summary>
    public DateTime unlockedDate;
}
