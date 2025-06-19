using Api.Attributes;
using Api.Models.Health;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;

namespace Api.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    [Route("api/v1/[controller]")]
    public class Health(IFastApiPredictionService fastApiPredictionService, IHolidayApiService holidayApiService, IWeatherService weatherService, ITrashImportService sensoringApiService) : ControllerBase
    {
        private readonly IFastApiPredictionService _fastApiPredictionService = fastApiPredictionService;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;
        private readonly IWeatherService _weatherService = weatherService;
        private readonly ITrashImportService _sensoringApiService = sensoringApiService;

        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new HealthStatus("Healthy", DateTime.UtcNow));
        }

        [HttpGet("fastapi")]
        public async Task<IActionResult> GetFastApiStatus()
        {
            try
            {
                var isHealthy = await _fastApiPredictionService.GetStatusAsync();
                if (isHealthy)
                    return Ok(new HealthStatus("Fast API is healthy", DateTime.UtcNow));
                else
                    return StatusCode(503, new HealthStatus("Fast API is not healthy", DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HealthStatus($"Error checking Fast API status: {ex.Message}", DateTime.UtcNow));
            }
        }
    }
}