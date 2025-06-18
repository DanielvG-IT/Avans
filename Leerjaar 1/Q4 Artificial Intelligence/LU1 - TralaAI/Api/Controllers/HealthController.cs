using Api.Attributes;
using Api.Models.Health;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    [Route("api/v1/[controller]")]
    public class Health : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            var healthDetails = new HealthDetails(
                Environment.MachineName,
                GetUptime(),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                GetMemoryUsage(),
                Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown"
            );

            var healthStatus = new HealthStatus("Healthy", DateTime.UtcNow, healthDetails);

            return Ok(healthStatus);
        }

        private static string GetUptime()
        {
            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            return $"{uptime.Days} days, {uptime.Hours} hours, {uptime.Minutes} minutes";
        }

        private static string GetMemoryUsage()
        {
            var memoryUsage = GC.GetTotalMemory(false) / (1024 * 1024); // Convert bytes to MB
            return $"{memoryUsage} MB";
        }
    }
}