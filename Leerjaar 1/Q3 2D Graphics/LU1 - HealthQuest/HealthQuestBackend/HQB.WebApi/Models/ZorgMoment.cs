namespace HQB.WebApi.Models;

public class ZorgMoment
{
    public Guid ID { get; set; }
    public required string Naam { get; set; }
    public string? Url { get; set; }
    public byte[]? Plaatje { get; set; }
    public int? TijdsDuurInMin { get; set; }
}