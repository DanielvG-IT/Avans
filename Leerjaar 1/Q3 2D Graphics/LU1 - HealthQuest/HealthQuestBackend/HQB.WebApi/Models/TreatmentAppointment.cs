public class TreatmentAppointment
{
    public Guid TreatmentID { get; set; }  // Foreign key to Treatment
    public Guid AppointmentID { get; set; }  // Foreign key to Appointment
    public int Sequence { get; set; }
}
