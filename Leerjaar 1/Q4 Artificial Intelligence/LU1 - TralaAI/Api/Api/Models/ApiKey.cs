namespace Api.Models;

public class ApiKey
{
    public int Id { get; set; }
    public Guid Key { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public required string Type { get; set; } // "Backend" or "FastApi"
}
