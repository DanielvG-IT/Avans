using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LitterController(ILitterRepository litterRepository, IFastApiPredictionService fastApiPredictionService) : ControllerBase
{
    private readonly ILitterRepository _litterRepository = litterRepository;
    private readonly IFastApiPredictionService _fastApiPredictionService = fastApiPredictionService;

    [HttpGet]
    public async Task<ActionResult<List<Litter>>> Get([FromQuery] LitterFilterDto filter)
    {
        var litters = await _litterRepository.GetFilteredAsync(filter);

        if (litters is null || litters.Count == 0)
            return NotFound("No litter found matching the filter.");

        return Ok(litters);
    }

    [HttpPost]
    public async Task<IActionResult> Predict()
    {
        var predictionResult = await _fastApiPredictionService.MakeLitterAmountPredictionAsync(DateTime.UtcNow); // TODO Update to use actual date input
        if (predictionResult is null)
            return BadRequest("Prediction failed. Please try again later.");

        if (predictionResult.Prediction is null)
            return NotFound("No prediction available for the given date.");

        return Ok(predictionResult);
    }


    [HttpPost]
    public async Task<IActionResult> Retrain()
    {
        var predictionResult = await _fastApiPredictionService.RetrainModelAsync();
        if (predictionResult is false)
            return StatusCode(StatusCodes.Status500InternalServerError, "Retraining failed. Please try again later.");

        return NoContent();
    }
}