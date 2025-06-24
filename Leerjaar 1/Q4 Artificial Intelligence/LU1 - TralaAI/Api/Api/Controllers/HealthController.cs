using Api.Attributes;
using Api.Models.Health;
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

        [HttpGet("fastapi/model")]
        public async Task<IActionResult> GetFastApiStatus([FromQuery] int cameraId)
        {
            try
            {
                var cameraModelStatus = await _fastApiPredictionService.GetModelStatus(cameraId);
                if (cameraModelStatus is null)
                    return StatusCode(503, new HealthStatus("Fast API model status not found", DateTime.UtcNow));

                return Ok(cameraModelStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HealthStatus($"Error checking Fast API status: {ex.Message}", DateTime.UtcNow));
            }
        }

        [HttpGet("holidayapi")]
        public async Task<IActionResult> GetHolidayApiStatus()
        {
            try
            {
                var date = DateTime.Now.Date;
                var IsHoliday = await _holidayApiService.IsHolidayAsync(date, "NL", date.Year.ToString());
                if (IsHoliday is null)
                    return StatusCode(503, new HealthStatus("Holiday API is not healthy", DateTime.UtcNow));
                else
                    return Ok(new HealthStatus("Holiday API is healthy", DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HealthStatus($"Error checking Holiday API status: {ex.Message}", DateTime.UtcNow));
            }
        }

        [HttpGet("weatherapi")]
        public async Task<IActionResult> GetWeatherApiStatus()
        {
            try
            {
                var isHealthy = await _weatherService.GetStatusAsync();
                if (isHealthy)
                    return Ok(new HealthStatus("Weather API is healthy", DateTime.UtcNow));
                else
                    return StatusCode(503, new HealthStatus("Weather API is not healthy", DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HealthStatus($"Error checking Weather API status: {ex.Message}", DateTime.UtcNow));
            }
        }

        [HttpGet("sensoringapi")]
        public async Task<IActionResult> GetSensoringApiStatus()
        {
            try
            {
                var isHealthy = await _sensoringApiService.GetStatusAsync();
                if (isHealthy)
                    return Ok(new HealthStatus("Sensoring API is healthy", DateTime.UtcNow));
                else
                    return StatusCode(503, new HealthStatus("Sensoring API is not healthy", DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HealthStatus($"Error checking Sensoring API status: {ex.Message}", DateTime.UtcNow));
            }
        }
    }
}