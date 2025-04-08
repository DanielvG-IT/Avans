using System;

/// <summary>
/// Represents a completed appointment in the system.
/// </summary>
[Serializable]
public class PersonalAppointments
{
    /// <summary>
    /// Gets or sets the unique identifier for the completed appointment.
    /// </summary>
    public string Id;

    /// <summary>
    /// Gets or sets the unique identifier of the patient associated with the completed appointment.
    /// </summary>
    public string PatientId;

    /// <summary>
    /// Gets or sets the unique identifier of the appointment that was completed.
    /// </summary>
    public string AppointmentId;

    /// <summary>
    /// Gets or sets the date and time when the appointment was completed.
    /// </summary>
    public string CompletedDate;

    /// <summary>
    /// Gets or sets if the question for the appointment is completed.
    /// </summary>
    public bool CompletedQuestion;
    /// <summary>
    /// Gets or sets the sequence of the appointment.
    /// </summary>
    public int Sequence;
}