namespace HQB.WebApi.Models;
public class Patient
{
    public Guid ID { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Guid GuardianID { get; set; }  // Foreign key to Guardian
    public Guid TreatmentID { get; set; }  // Foreign key to Treatment
    public Guid DoctorID { get; set; }  // Foreign key to Doctor
    public required string Avatar { get; set; }  // Avatar image URL or path
}