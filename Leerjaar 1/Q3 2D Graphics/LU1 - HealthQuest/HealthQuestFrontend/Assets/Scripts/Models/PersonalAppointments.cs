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
    public string id;

    /// <summary>
    /// Gets or sets the unique identifier of the patient associated with the completed appointment.
    /// </summary>
    public string patientID;

    /// <summary>
    /// Gets or sets the unique identifier of the appointment that was completed.
    /// </summary>
    public string appointmentID;

    /// <summary>
    /// Gets or sets the date and time when the appointment is scheduled.
    /// </summary>
    public string appointmentDate;

    /// <summary>
    /// Gets or sets the date and time when the appointment was completed.
    /// </summary>
    public string completedDate;

    /// <summary>
    /// Gets or sets if the question for the appointment is completed.
    /// </summary>
    public bool completedQuestion;
    /// <summary>
    /// Gets or sets the sequence of the appointment.
    /// </summary>
    public int sequence;
}