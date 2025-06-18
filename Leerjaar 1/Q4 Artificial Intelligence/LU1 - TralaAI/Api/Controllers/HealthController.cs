using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Health : ControllerBase
    {
        [HttpGet]
        [Route("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                details = new
                {
                    server = Environment.MachineName,
                    uptime = GetUptime(),
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    memoryUsage = GetMemoryUsage(),
                    version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown"
                }
            });
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