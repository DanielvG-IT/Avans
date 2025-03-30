using System;

#nullable enable

/// <summary>
/// Represents an appointment with details such as ID, Name, Url, Image, and Duration.
/// </summary>
[Serializable]
public class Appointment
{
    /// <summary>
    /// Gets or sets the unique identifier for the appointment.
    /// </summary>
    public Guid id { get; set; }

    /// <summary>
    /// Gets or sets the name of the appointment.
    /// </summary>
    public string name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL associated with the appointment.
    /// </summary>
    public string? url { get; set; }

    /// <summary>
    /// Gets or sets the image associated with the appointment.
    /// </summary>
    public byte[]? image { get; set; }

    /// <summary>
    /// Gets or sets the duration of the appointment in minutes.
    /// </summary>
    public int durationInMinutes { get; set; }
}
