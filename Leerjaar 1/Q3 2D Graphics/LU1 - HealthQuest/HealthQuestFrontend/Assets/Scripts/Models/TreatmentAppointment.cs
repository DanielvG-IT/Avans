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
    public Guid treatmentID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the appointment.
    /// This is a foreign key to the Appointment entity.
    /// </summary>
    public Guid appointmentID { get; set; }

    /// <summary>
    /// Gets or sets the sequence number of the treatment appointment.
    /// </summary>
    public int sequence { get; set; }
}
