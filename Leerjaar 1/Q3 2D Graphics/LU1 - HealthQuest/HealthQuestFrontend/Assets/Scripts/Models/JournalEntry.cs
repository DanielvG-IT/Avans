using System;

/// <summary>
/// Represents a journal entry for a patient.
/// </summary>
[Serializable]
public class JournalEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for the journal entry.
    /// </summary>
    public string id;

    /// <summary>
    /// Gets or sets the unique identifier for the patient associated with the journal entry.
    /// </summary>
    #nullable enable
    public string? patientID;  // Foreign key to Patient

    /// <summary>
    /// Gets or sets the unique identifier for the guardian associated with the journal entry.
    /// </summary>
    public string? guardianID;

    /// <summary>
    /// Gets or sets the date of the journal entry.
    /// </summary>
    public DateTime date;

    /// <summary>
    /// Gets or sets the title of the journal entry.
    /// </summary>
    public string? title;

    /// <summary>
    /// Gets or sets the content of the journal entry.
    /// </summary>
    public string? content;

    /// <summary>
    /// Gets or sets the rating of the journal entry.
    /// </summary>
    /// <remarks>
    /// The rating is a value between 1 and 10.
    /// </remarks>
    public int rating;
}