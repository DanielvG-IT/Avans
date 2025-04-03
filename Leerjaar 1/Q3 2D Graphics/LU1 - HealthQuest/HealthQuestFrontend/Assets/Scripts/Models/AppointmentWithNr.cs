using System;

/// <summary>
/// Represents an appointment with an associated appointment number.
/// Inherits from the <see cref="Appointment"/> class.
/// </summary>
[Serializable]
public class AppointmentWithNr : Appointment
{
    /// <summary>
    /// Gets or sets the number representing the position of the appointment within the treatment plan.
    /// </summary>
    public int appointmentNr;
}