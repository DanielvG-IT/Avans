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
    public async Task<IActionResult> Predict([FromQuery] int amountOfDays, [FromQuery] string location)
    {
        if (amountOfDays <= 0)
            return BadRequest("Amount of days must be greater than 0.");
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location cannot be empty.");

        var modelInputs = new List<Input>();

        for (var i = 0; i < amountOfDays; i++)
        {
            var uniqueDates = modelInputs.Select(input => DateTime.UtcNow.AddDays(input.DayOfWeek).Date).Distinct().ToList();
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i).Date);

            var holidayDictionary = new Dictionary<DateOnly, bool>();
            foreach (var date in uniqueDates)
                holidayDictionary[DateOnly.FromDateTime(date)] = await _holidayApiService.IsHolidayAsync(date, "NL");

            var weatherDictionary = new Dictionary<DateOnly, FastApiWeatherRequirements>();
            foreach (var date in uniqueDates)
            {
                var dateWeatherData = await _weatherService.GetWeatherAsync(DateOnly.FromDateTime(date));
                weatherDictionary[DateOnly.FromDateTime(date)] = dateWeatherData;
            }

            var dayOfWeek = (int)DateTime.UtcNow.AddDays(i).DayOfWeek;
            var month = DateTime.UtcNow.AddDays(i).Month;
            var holiday = holidayDictionary.TryGetValue(currentDate, out bool value) && value;
            var weather = weatherDictionary.TryGetValue(currentDate, out FastApiWeatherRequirements weatherData) && weatherData?.Condition is not null
                ? Enum.TryParse<WeatherCategory>(weatherData.Condition, true, out var parsedWeather)
                    ? (int)parsedWeather : 0 : 0;
            var temperatureCelcius = weatherData?.Temperature is not null ? (int)weatherData.Temperature : 20;
            var isWeekend = dayOfWeek == 0 || dayOfWeek == 6; // Sunday or Saturday

            modelInputs.Add(new Input
            {
                DayOfWeek = dayOfWeek,
                Month = month,
                Holiday = holiday,
                Weather = weather,
                TemperatureCelcius = temperatureCelcius,
                IsWeekend = isWeekend,
                Label = $"{DateTime.UtcNow.AddDays(i):yyyy-MM-dd}"
            });
        }

        var predictionRequest = new PredictionRequest { ModelIndex = location, Inputs = modelInputs };
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
    public async Task<IActionResult> Retrain()
    {
        var retrainResult = await _fastApiPredictionService.RetrainModelAsync();
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