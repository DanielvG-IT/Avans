namespace HQB.WebApi.Models;
/// <summary>
/// Represents an appointment with details such as ID, Name, Url, Image, and Duration.
/// </summary>
public class AppointmentWithNr : Appointment
{
    /// <summary>
    /// Gets or sets the appointment number.
    /// </summary>
    public int AppointmentNr { get; set; }
}
