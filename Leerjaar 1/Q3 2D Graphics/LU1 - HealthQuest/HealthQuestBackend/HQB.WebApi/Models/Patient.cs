namespace HQB.WebApi.Models;

public class Patient
{
    public int ID { get; set; }
    public required string Voornaam { get; set; }
    public required string Achternaam { get; set; }
    public int OuderVoogdID { get; set; }
    public required OuderVoogd OuderVoogd { get; set; }
    public int TrajectID { get; set; }
    public required Traject Traject { get; set; }
    public int? ArtsID { get; set; }
    public Arts? Arts { get; set; }
}