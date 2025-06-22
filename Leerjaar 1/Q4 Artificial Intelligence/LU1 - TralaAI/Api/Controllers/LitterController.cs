using Api.Models;
using Api.Attributes;
using Api.Interfaces;
using Api.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[ApiKeyAuth]
[Route("api/v1/[controller]")]
public class LitterController(ILitterRepository litterRepository, IFastApiPredictionService fastApiPredictionService, IHolidayApiService holidayApiService, IWeatherService weatherService, IDTOService dTOService) : ControllerBase
{
    private readonly ILitterRepository _litterRepository = litterRepository;
    private readonly IFastApiPredictionService _fastApiPredictionService = fastApiPredictionService;
    private readonly IHolidayApiService _holidayApiService = holidayApiService;
    private readonly IWeatherService _weatherService = weatherService;
    private readonly IDTOService _dTOService = dTOService;

    [HttpGet]
    public async Task<ActionResult<List<Litter>>> Get([FromQuery] LitterFilterDto filter)
    {
        var litters = await _litterRepository.GetFilteredAsync(filter);

        if (litters is null || litters.Count == 0)
            return NotFound("No litter found matching the filter.");

        return Ok(litters);
    }

    [HttpGet("cameras")]
    public async Task<ActionResult> GetCameras()
    {
        var cameras = await _litterRepository.GetCamerasAsync();
        if (cameras is null || cameras.Count == 0)
            return NotFound("No cameras found.");

        return Ok(cameras);
    }

    [HttpPost("predict")]
    public async Task<IActionResult> Predict([FromQuery] int amountOfDays, [FromQuery] int CameraId)
    {
        if (amountOfDays <= 0)
            return BadRequest("Amount of days must be greater than 0.");
        if (CameraId < 0 || CameraId > 5)
            return BadRequest("Camera ID must be between 0 and 5.");

        var today = DateTime.UtcNow.Date;
        var dates = Enumerable.Range(0, amountOfDays).Select(i => today.AddDays(i)).ToList();

        // Fetch holidays in parallel for all dates
        var holidayTasks = dates.Select(date => _holidayApiService.IsHolidayAsync(date, "NL", date.Year.ToString())).ToArray();
        var holidays = await Task.WhenAll(holidayTasks);

        var weatherForecasts = await _weatherService.GetWeatherAsync(amountOfDays);
        if (weatherForecasts is null || weatherForecasts.Count != amountOfDays)
            return BadRequest("Invalid weather data received. Please try again later.");

        var validDateInfos = dates
            .Select((date, idx) => new
            {
                Date = date,
                Holiday = holidays[idx],
                Weather = weatherForecasts.FirstOrDefault(w => w.Date.Date == date.Date)
            })
            .Where(info => info.Weather is not null)
            .Select(info =>
            {
                var weatherEnum = info.Weather is not null ? _dTOService.GetWeatherCategory(info.Weather.Condition) : null;
                var weatherCondition = weatherEnum is not null ? _dTOService.GetWeatherCategoryIndex(weatherEnum) : null;
                return new
                {
                    info.Date,
                    info.Holiday,
                    info.Weather,
                    WeatherCondition = weatherCondition
                };
            })
            .Where(info => info.WeatherCondition is not null)
            .ToList();

        if (validDateInfos.Count != amountOfDays)
            return BadRequest("Couldn't create model inputs due to missing or invalid weather data.");

        var modelInputs = validDateInfos.Select(info =>
            new Input
            {
                DayOfWeek = (int)info.Date.DayOfWeek,
                Month = info.Date.Month,
                Holiday = info.Holiday ?? false,
                Weather = info.WeatherCondition ?? 0,
                TemperatureCelcius = info.Weather is not null ? (int)info.Weather.Temperature : 0,
                IsWeekend = info.Date.DayOfWeek == DayOfWeek.Saturday || info.Date.DayOfWeek == DayOfWeek.Sunday,
                Label = info.Date.ToString("yyyy-MM-dd")
            }
        ).ToList();

        var predictionRequest = new PredictionRequest
        {
            CameraId = CameraId,
            Inputs = [.. modelInputs.Cast<Input>()]
        };

        var predictionResult = await _fastApiPredictionService.MakeLitterAmountPredictionAsync(predictionRequest);

        if (predictionResult is null)
            return BadRequest("Prediction service returned null. Please try again later.");
        if (predictionResult is null || predictionResult.Count == 0)
            return NotFound("No predictions found for the given inputs.");
        if (predictionResult.Any(p => p is null))
            return BadRequest("Invalid prediction data received. Please try again later.");
        if (predictionResult.Count != amountOfDays)
            return BadRequest($"Expected {amountOfDays} predictions, but got {predictionResult.Count}.");

        return Ok(predictionResult);
    }

    [HttpPost("retrain")]
    public async Task<IActionResult> Retrain([FromQuery] int CameraId)
    {
        var retrainResult = await _fastApiPredictionService.RetrainModelAsync(CameraId);
        if (!retrainResult)
            return BadRequest($"Retrain failed. Please try again later.");

        return Ok("Model retrained successfully.");
    }

    [HttpGet("latest")]
    public async Task<ActionResult<List<Litter>>> GetLatest([FromQuery] int amount = 100)
    {
        var litters = await _litterRepository.GetLatestAsync(amount);

        if (litters is null || litters.Count == 0)
            return NotFound("No latest litter records found.");

        return Ok(litters);
    }

    [HttpGet("amount-per-location")]
    public async Task<ActionResult<LitterAmountCamera>> GetAmountPerCamera()
    {
        var amountPerLocation = await _litterRepository.GetAmountPerCameraAsync();

        if (amountPerLocation is null)
            return NotFound("No litter amount data found for the specified camera.");

        return Ok(amountPerLocation);
    }
}