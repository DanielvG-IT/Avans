namespace Blazor.Models.Health
{
    public record HealthStatus(string Status, DateTime Timestamp, HealthDetails Details);
}