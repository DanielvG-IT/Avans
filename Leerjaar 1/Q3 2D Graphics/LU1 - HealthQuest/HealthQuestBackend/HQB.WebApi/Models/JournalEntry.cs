namespace HQB.WebApi.Models;
/// <summary>
/// Represents a journal entry for a patient.
/// </summary>
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
    /// Gets or sets the date of the journal entry as string.
    /// </summary>
    public required string Date { get; set; }

    /// <summary>
    /// Gets or sets the title of the journal entry.
    /// </summary>
    /// <remarks>
    /// The title is required and should be descriptive of the journal entry.
    /// </remarks>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the journal entry.
    /// </summary>
    /// <remarks>
    /// The content is required and should contain the details of the journal entry.
    /// </remarks>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the rating.
    /// </summary>
    /// remarks>
    /// The rating is an integer value that represents the quality or satisfaction level of the journal entry.
    /// </remarks>
    public int Rating { get; set; }
}