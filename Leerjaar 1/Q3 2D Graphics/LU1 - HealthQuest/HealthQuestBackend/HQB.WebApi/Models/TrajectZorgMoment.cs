namespace HQB.WebApi.Models;

public class TrajectZorgMoment
{
    public Guid TrajectID { get; set; }
    public required Traject Traject { get; set; }
    public Guid ZorgMomentID { get; set; }
    public required ZorgMoment ZorgMoment { get; set; }
    public int Volgorde { get; set; }
}