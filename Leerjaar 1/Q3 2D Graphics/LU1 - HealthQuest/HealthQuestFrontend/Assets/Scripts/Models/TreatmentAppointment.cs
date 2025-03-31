using System;

/// <summary>
/// Represents an appointment for a specific treatment.
/// </summary>
[Serializable]
public class TreatmentAppointment
{
    /// <summary>
    /// Gets or sets the unique identifier for the treatment.
    /// This is a foreign key to the Treatment entity.
    /// </summary>
    public string treatmentID;

    /// <summary>
    /// Gets or sets the unique identifier for the appointment.
    /// This is a foreign key to the Appointment entity.
    /// </summary>
    public string appointmentID;

    /// <summary>
    /// Gets or sets the sequence number of the treatment appointment.
    /// </summary>
    public int sequence;
}
