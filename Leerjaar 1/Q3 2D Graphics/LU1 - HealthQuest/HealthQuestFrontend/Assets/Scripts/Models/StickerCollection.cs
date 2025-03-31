using System;

/// <summary>
/// Represents a collection of stickers associated with a patient.
/// </summary>
[Serializable]
public class StickerCollection
{
  /// <summary>
  /// Gets or sets the unique identifier for the sticker collection.
  /// </summary>
  /// <value>The unique identifier for the sticker collection.</value>
  public string id; // PK

  /// <summary>
  /// Gets or sets the unique identifier for the patient.
  /// </summary>
  /// <value>The unique identifier for the patient.</value>
  public string patientID { get; set; } // FK

  /// <summary>
  /// Gets or sets the unique identifier for the sticker.
  /// </summary>
  /// <value>The unique identifier for the sticker.</value>
  public string stickerID; // FK

  /// <summary>
  /// Gets or sets the date when the sticker was unlocked.
  /// </summary>
  /// <value>The date when the sticker was unlocked.</value>
  public DateTime unlockedDate;
}