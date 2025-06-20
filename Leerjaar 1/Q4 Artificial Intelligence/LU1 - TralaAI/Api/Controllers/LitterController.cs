using Api.Models;
using Api.Attributes;
using Api.Interfaces;
using Api.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[ApiKeyAuth]
[Route("api/v1/[controller]")]
public class LitterController(ILitterRepository litterRepository, IFastApiPredictionService fastApiPredictionService, IHolidayApiService holidayApiService, IWeatherService weatherService) : ControllerBase
{
    private readonly ILitterRepository _litterRepository = litterRepository;
    private readonly IFastApiPredictionService _fastApiPredictionService = fastApiPredictionService;
    private readonly IHolidayApiService _holidayApiService = holidayApiService;
    private readonly IWeatherService _weatherService = weatherService;

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

        // Optionally, fetch weather data in parallel for all dates
        // For now, use placeholders as before
        var modelInputs = dates.Select((date, idx) =>
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var month = date.Month;
            var holiday = holidays[idx];
            var isWeekend = dayOfWeek == 0 || dayOfWeek == 6; // Sunday or Saturday

            return new Input
            {
                DayOfWeek = dayOfWeek,
                Month = month,
                Holiday = holiday,
                Weather = 1, // Placeholder for weather condition index
                TemperatureCelcius = 20, // Placeholder temperature
                IsWeekend = isWeekend,
                Label = date.ToString("yyyy-MM-dd")
            };
        }).ToList();

        var predictionRequest = new PredictionRequest
        {
            ModelIndex = location.ToString(),
            Inputs = modelInputs
        };

        var predictionResult = await _fastApiPredictionService.MakeLitterAmountPredictionAsync(predictionRequest);

        if (predictionResult is null)
            return BadRequest("Prediction failed. Please try again later.");
        if (predictionResult.Predictions is null || predictionResult.Predictions.Count == 0)
            return NotFound("No predictions found for the given inputs.");
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