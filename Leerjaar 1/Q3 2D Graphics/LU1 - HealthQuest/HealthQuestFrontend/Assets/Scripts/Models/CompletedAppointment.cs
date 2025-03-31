using System;

/// <summary>
/// Represents a completed appointment in the system.
/// </summary>
[Serializable]
public class CompletedAppointment
{
    /// <summary>
    /// Gets or sets the unique identifier for the completed appointment.
    /// </summary>
    public string id;

    /// <summary>
    /// Gets or sets the unique identifier of the patient associated with the completed appointment.
    /// </summary>
    public string patientId;

    /// <summary>
    /// Gets or sets the unique identifier of the appointment that was completed.
    /// </summary>
    public string appointmentId;

    /// <summary>
    /// Gets or sets the date and time when the appointment was completed.
    /// </summary>
    public DateTime completedDate;
}
