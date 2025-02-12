using CoreLink.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Controllers;

[ApiController]
[Route("Environments")]
public class EnvironmentsController(ILogger<EnvironmentsController> logger) : ControllerBase
{
    //private readonly ILogger<EnvironmentsController> _logger = logger;

    // Getting Environment
    [HttpGet]
    public IActionResult Get()
    {
        var environments = EnvironmentRepository.GetEnvironments();
        return Ok(environments);
    }

    [HttpGet("{name}")]
    public IActionResult Get(string name)
    {
        var environment = EnvironmentRepository.GetEnvironment(name);

        if (environment == null)
        {
            return NotFound($"Environment with name '{name}' not found.");
        }

        return Ok(environment);
    }


    // Creating a new environment
    [HttpPost]
    public IActionResult Post([FromBody] Environment2D payload)
    {
        if (string.IsNullOrEmpty(payload.name))
        {
            return BadRequest("name is required.");
        }
        if (string.IsNullOrEmpty(payload.maxLength))
        {
            return BadRequest("maxLength is required.");
        }
        if (string.IsNullOrEmpty(payload.maxHeight))
        {
            return BadRequest("maxHeight is required.");
        }

        // Save the environment to the ...MEDIUM...
        int result = EnvironmentRepository.AddEnvironment(payload);

        return result switch
        {
            201 => Created("", new { Message = $"Created environment {payload.name}"}),
            400 => BadRequest("Environment already exists."),
            _ => StatusCode(500, "An error occurred while processing your request.")
        };
    }

    // Updating an environment
    [HttpPut("{name}")]
    public IActionResult Put(string name, [FromBody] Environment2D payload)
    {
        var result = EnvironmentRepository.UpdateEnvironment(name, payload);

        return result;
    }


    // Deleting an environment
    [HttpDelete("{name}")]
    public IActionResult Delete(string name)
    {
        var result = EnvironmentRepository.RemoveEnvironment(name);

        return result switch
        {
            204 => NoContent(),
            404 => BadRequest("Environment not found."),
            _ => StatusCode(500, "An error occurred while processing your request.")
        };
    }
}
