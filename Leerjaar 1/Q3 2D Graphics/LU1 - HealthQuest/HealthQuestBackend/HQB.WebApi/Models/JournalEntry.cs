namespace HQB.WebApi.Models;
public class JournalEntry
{
    public Guid ID { get; set; }
    public Guid PatientID { get; set; }  // Foreign key to Patient
    public DateTime Date { get; set; }
    public required string Content { get; set; }
    public Guid GuardianID { get; set; }  // Foreign key to Guardian
}