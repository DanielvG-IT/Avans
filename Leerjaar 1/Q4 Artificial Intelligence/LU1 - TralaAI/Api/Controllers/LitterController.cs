using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Attributes;
using Api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Controllers;

[ApiController]
[ApiKeyAuth]
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

    [HttpPost("predict")]
    public async Task<IActionResult> Predict(List<PredictionRequest> dtos)
    {
        if (dtos is null || dtos.Count == 0)
            return BadRequest("Prediction request models cannot be null or empty.");

        var predictionResult = await _fastApiPredictionService.MakeLitterAmountPredictionAsync(dtos);
        if (predictionResult is null)
            return BadRequest("Prediction failed. Please try again later.");

        if (predictionResult.Predictions is null || predictionResult.Predictions.Count == 0)
            return NotFound("No prediction available for the given input.");

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
}