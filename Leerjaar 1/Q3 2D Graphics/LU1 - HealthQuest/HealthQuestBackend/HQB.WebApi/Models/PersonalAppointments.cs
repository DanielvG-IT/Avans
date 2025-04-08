namespace HQB.WebApi.Models;
/// <summary>
/// Represents a personal appointment in the system.
/// </summary>
public class PersonalAppointments
{
    /// <summary>
    /// Gets or sets the unique identifier for the completed appointment.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient associated with the completed appointment.
    /// </summary>
    public Guid PatientID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the appointment that was completed.
    /// </summary>
    public Guid AppointmentID { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the appointment is scheduled.
    /// </summary>
    public DateTime? AppointmentDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the appointment was completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Gets or sets if the question for the appointment is completed.
    /// </summary>
    public bool CompletedQuestion { get; set; }

    /// <summary>
    /// Gets or sets the sequence of the appointment.
    /// </summary>
    public int Sequence { get; set; }

}
