using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreLink.WebApi.Controllers;

[ApiController]
[Route("Environments")]
public class EnvironmentsController(ILogger<EnvironmentsController> logger, IEnvironmentRepository environmentRepository, IObjectRepository objectRepository) : ControllerBase
{
    private readonly ILogger<EnvironmentsController> _logger = logger;
    private readonly IEnvironmentRepository _environmentRepository = environmentRepository;


    // Getting Environment
    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var environments = await _environmentRepository.ReadAsync();

        if (!environments.Any())
        {
            return NotFound();
        }

        return Ok(environments);
    }

    [HttpGet("{Id}", Name = "ReadEnvironmentsByGuid")]
    public async Task<ActionResult<Environment2D>> Get(Guid Id)
    {
        var environment = await _environmentRepository.ReadAsync(Id);

        if (environment == null)
        {
            return NotFound();
        }

        return Ok(environment);
    }


    // Creating a new environment
    [HttpPost(Name = "CreateEnvironment")]
    public IActionResult Post(Environment2D newEnvironment)
    {
        newEnvironment.Id = Guid.NewGuid();
        _environmentRepository.CreateAsync(newEnvironment);

        return CreatedAtRoute("ReadEnvironmentsByGuid", new { Id = newEnvironment.Id }, newEnvironment);
    }

    // Updating an environment
    [HttpPut("{Id}", Name = "UpdateEnvironmentByGuid")]
    public IActionResult Put(Guid Id, Environment2D Environment)
    {
        // TODO Check if the environment exists
        var environment = _environmentRepository.ReadAsync(Id);
        if (environment == null) { return NotFound(); }

        _environmentRepository.UpdateAsync(Environment);

        return Ok();
    }


    // Deleting an environment
    [HttpDelete("{Id}", Name = "DeleteEnvironmentByGuid")]
    public IActionResult Delete(Guid Id)
    {
        // TODO Improve Check if the environment exists
        var environment = _environmentRepository.ReadAsync(Id);
        if (environment == null) { return NotFound(); }

        _environmentRepository.DeleteAsync(Id);

        return NoContent();
    }










    // Get objects from environment
    [HttpGet("{EnvironmentId}/objects", Name = "GetObjectsByEnvironmentGuid")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetObjects(Guid EnvironmentId)
    {
        // TODO Check if the environment exists
        var environment = _environmentRepository.ReadAsync(Id);
        if (environment == null) { return NotFound(); }

        _environmentRepository.UpdateAsync(Environment);

        return Ok();
    }

    // Get objects from environment
    [HttpGet("{EnvironmentId}/objects/{ObjectId}", Name = "GetObjectByObjectGuid")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetObject(Guid ObjectId, Guid EnvironmentId )
    {
        // TODO Check if the environment exists
        var environment = _environmentRepository.ReadAsync(Id);

        if (environment == null) { return NotFound(); }

        return Ok();
    }



}