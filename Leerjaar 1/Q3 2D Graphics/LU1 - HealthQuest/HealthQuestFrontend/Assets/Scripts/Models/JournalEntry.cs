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
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the patient associated with the journal entry.
    /// </summary>
    public Guid? PatientID { get; set; }  // Foreign key to Patient

    /// <summary>
    /// Gets or sets the unique identifier for the guardian associated with the journal entry.
    /// </summary>
    public Guid? GuardianID { get; set; }

    /// <summary>
    /// Gets or sets the date of the journal entry.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the content of the journal entry.
    /// </summary>
    public string Content { get; set; }
}