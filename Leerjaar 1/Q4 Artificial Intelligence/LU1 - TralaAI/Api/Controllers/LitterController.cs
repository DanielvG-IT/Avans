using Api.Models;
using Api.Attributes;
using Api.Interfaces;
using Api.Models.Enums;
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

    [HttpPost("predict")]
    public async Task<IActionResult> Predict([FromQuery] int amountOfDays, [FromQuery] int location)
    {
        if (amountOfDays <= 0)
            return BadRequest("Amount of days must be greater than 0.");
        if (location < 0 || location > 5)
            return BadRequest("Location must be between 0 and 5.");

        var today = DateTime.UtcNow.Date;
        var dates = Enumerable.Range(0, amountOfDays).Select(i => today.AddDays(i)).ToList();

        // Fetch holidays in parallel for all dates
        var holidayTasks = dates.Select(date => _holidayApiService.IsHolidayAsync(date, "NL", date.Year.ToString())).ToArray();
        var holidays = await Task.WhenAll(holidayTasks);

        var weatherForecasts = await _weatherService.GetWeatherAsync(amountOfDays);
        if (weatherForecasts is null || weatherForecasts.Count != amountOfDays)
            return BadRequest("Invalid weather data received. Please try again later.");


        var modelInputs = dates.Select((date, idx) =>
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var month = date.Month;
            var holiday = holidays[idx];
            var isWeekend = dayOfWeek == 0 || dayOfWeek == 6; // Sunday or Saturday
            var weather = weatherForecasts[idx];

            var weatherCondition = _dTOService.GetWeatherCategoryIndex(weather.Condition);
            if (weatherCondition is null)
                return null;

            return new Input
            {
                DayOfWeek = dayOfWeek,
                Month = month,
                Holiday = holiday,
                Weather = (int)weatherCondition,
                TemperatureCelcius = (int)weather.Temperature,
                IsWeekend = isWeekend,
                Label = date.ToString("yyyy-MM-dd")
            };
        }).ToList();

        // Check for null inputs
        if (modelInputs is null || modelInputs.Count == 0)
            return BadRequest("No valid weather data received. Please try again later.");
        if (modelInputs.Any(input => input is null))
            return BadRequest("Invalid weather data received. Please try again later.");
        modelInputs = [.. modelInputs.Where(input => input is not null)];
        if (modelInputs.Count != amountOfDays)
            return BadRequest($"Expected {amountOfDays} inputs, but got {modelInputs.Count}.");

        var predictionRequest = new PredictionRequest
        {
            ModelIndex = location.ToString(),
            Inputs = [.. modelInputs.Cast<Input>()]
        };

        var predictionResult = await _fastApiPredictionService.MakeLitterAmountPredictionAsync(predictionRequest);

        if (predictionResult is null)
            return BadRequest("Prediction service returned null. Please try again later.");
        if (predictionResult.Predictions is null || predictionResult.Predictions.Count == 0)
            return NotFound("No predictions found for the given inputs.");
        if (predictionResult.Predictions.Any(p => p is null))
            return BadRequest("Invalid prediction data received. Please try again later.");
        if (predictionResult.Predictions.Count != amountOfDays)
            return BadRequest($"Expected {amountOfDays} predictions, but got {predictionResult.Predictions.Count}.");

        return Ok(predictionResult);
    }

    [HttpPost("retrain")]
    public async Task<IActionResult> Retrain([FromQuery] string cameraLocation)
    {
        var retrainResult = await _fastApiPredictionService.RetrainModelAsync(cameraLocation);
        if (!retrainResult)
            return BadRequest($"Retrain failed. Please try again later.");

        return Ok("Model retrained successfully.");
    }

    [HttpGet("latest")]
    public async Task<ActionResult<List<Litter>>> GetLatest([FromQuery] int? amount)
    {
        var litters = await _litterRepository.GetLatestAsync(amount);

        if (litters is null || litters.Count == 0)
            return NotFound("No latest litter records found.");

        return Ok(litters);
    }

    [HttpGet("amount-per-location")]
    public async Task<ActionResult<LitterTypeAmount?>> GetAmountPerLocation()
    {
        var amountPerLocation = await _litterRepository.GetAmountPerLocationAsync();

        if (amountPerLocation is null)
            return NotFound("No litter amount data found for the specified location.");

        return Ok(amountPerLocation);
    }

    [HttpGet("history")]
    public async Task<ActionResult<object>> GetHistory()
    {
        try
        {
            var amountPerLocationTask = _litterRepository.GetAmountPerLocationAsync();
            var historyTask = _litterRepository.GetLatestAsync(100);

            await Task.WhenAll(amountPerLocationTask, historyTask);

            var amountPerLocation = await amountPerLocationTask ?? new LitterTypeAmount();
            var history = await historyTask ?? [];

            var response = new LitterHistoryResponse
            {
                AmountPerLocation = amountPerLocation,
                History = history,
                Metadata = new LitterHistoryMetadata
                {
                    RetrievedAt = DateTime.UtcNow,
                    RecordCount = history.Count
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500, title: "An unexpected error occurred while retrieving litter history.");
        }
    }
}