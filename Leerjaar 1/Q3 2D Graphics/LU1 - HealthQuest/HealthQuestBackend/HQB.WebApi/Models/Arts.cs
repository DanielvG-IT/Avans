namespace HQB.WebApi.Models;

public class Arts
{
    public Guid ID { get; set; }
    public required string Naam { get; set; }
    public required string Specialisatie { get; set; }
    public ICollection<Patient>? Patients { get; set; }
}