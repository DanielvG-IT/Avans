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
    public Guid id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the patient associated with the journal entry.
    /// </summary>
    public Guid? patientID { get; set; }  // Foreign key to Patient

    /// <summary>
    /// Gets or sets the unique identifier for the guardian associated with the journal entry.
    /// </summary>
    public Guid? guardianID { get; set; }

    /// <summary>
    /// Gets or sets the date of the journal entry.
    /// </summary>
    public DateTime date { get; set; }

    /// <summary>
    /// Gets or sets the content of the journal entry.
    /// </summary>
    public string content { get; set; }
}