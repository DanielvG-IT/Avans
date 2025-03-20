namespace HQB.WebApi.Models;

public class Patient
{
    public Guid ID { get; set; }
    public required string Voornaam { get; set; }
    public required string Achternaam { get; set; }
    public required string OuderVoogdId { get; set; }
    public required OuderVoogd OuderVoogd { get; set; }
    public Guid TrajectID { get; set; }
    public required Traject Traject { get; set; }
    public Guid? ArtsID { get; set; }
    public Arts? Arts { get; set; }
}