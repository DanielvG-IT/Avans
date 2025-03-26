namespace HQB.WebApi.Models;
public class CompletedAppointment
{
  public Guid Id { get; set; } // Primary Key
  public Guid PatientId { get; set; } // Foreign Key
  public Guid AppointmentId { get; set; } // Foreign Key
  public DateTime CompletedDate { get; set; }
}
