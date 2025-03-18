namespace HQB.WebApi.Models;

public class TrajectZorgMoment
{
    public int TrajectID { get; set; }
    public required Traject Traject { get; set; }
    public int ZorgMomentID { get; set; }
    public required ZorgMoment ZorgMoment { get; set; }
    public int Volgorde { get; set; }
}