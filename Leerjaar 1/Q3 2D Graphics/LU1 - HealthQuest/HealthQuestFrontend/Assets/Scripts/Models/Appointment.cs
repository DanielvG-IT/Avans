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
    public string? id;

    /// <summary>
    /// Gets or sets the name of the appointment.
    /// </summary>
    public string name = string.Empty;

    /// <summary>
    /// Gets or sets the description of the appointment.
    /// </summary>
    public string description = string.Empty;

    /// <summary>
    /// Gets or sets the URL associated with the appointment.
    /// </summary>
    public string? url;

    /// <summary>
    /// Gets or sets the image associated with the appointment.
    /// </summary>
    public byte[]? image;

    /// <summary>
    /// Gets or sets the duration of the appointment in minutes.
    /// </summary>
    public int durationInMinutes;
}