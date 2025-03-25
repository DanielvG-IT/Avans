public class Appointment
{
    public Guid ID { get; set; }
    public required string Name { get; set; }
    public string? Url { get; set; }
    public byte[]? Image { get; set; }
    public int DurationInMinutes { get; set; }
}
