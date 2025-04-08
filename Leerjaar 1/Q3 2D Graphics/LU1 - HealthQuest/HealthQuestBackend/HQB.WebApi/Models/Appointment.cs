namespace HQB.WebApi.Models;
/// <summary>
/// Represents an appointment with details such as ID, Name, Url, Image, and Duration.
/// </summary>
public class Appointment
{
    /// <summary>
    /// Gets or sets the unique identifier for the appointment.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the name of the appointment.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the appointment.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the URL associated with the appointment.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the image associated with the appointment.
    /// </summary>
    public byte[]? Image { get; set; }

    /// <summary>
    /// Gets or sets the duration of the appointment in minutes.
    /// </summary>
    public int DurationInMinutes { get; set; }

    /// <summary>
    /// Gets or sets the treatment ID associated with the appointment.
    /// This is a foreign key to the treatment table.
    /// </summary>
    public Guid TreatmentID { get; set; }

    /// <summary>
    /// Gets or sets the sequence number in the treatment.
    /// </summary>
    public int Sequence { get; set; }
}
