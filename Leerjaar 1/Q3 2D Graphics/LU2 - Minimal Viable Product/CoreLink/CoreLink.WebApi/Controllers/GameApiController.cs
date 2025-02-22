using CoreLink.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Controllers;

[ApiController]
[Route("environments")]
public class GameApiController : ControllerBase
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IObjectRepository _objectRepository;
    private readonly IAuthenticationService _authenticationService;

    public GameApiController(IAuthenticationService authenticationService, IEnvironmentRepository environmentRepository, IObjectRepository objectRepository)
    {
        _environmentRepository = environmentRepository;
        _objectRepository = objectRepository;
        _authenticationService = authenticationService;
    }

    // HTTP METHODES

    [HttpGet(Name = "ReadEnvironmentsFromUser")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        var environments = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (!environments.Any())
            return NotFound("No environments found for the current user.");

        return Ok(environments);
    }

    // TODO Maybe redundant methode (because unity already has all environments from user)
    // [HttpGet("{environmentId}", Name = "ReadEnvironmentById")]
    // public async Task<ActionResult<Environment2D>> Get(Guid environmentId)
    // {
    //     var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

    //     if (environmentId == Guid.Empty)
    //         return BadRequest("The environment ID is not valid.");

    //     if (string.IsNullOrEmpty(loggedInUser))
    //         return Unauthorized("User is not authenticated.");

    //     var environment = await _environmentRepository.GetEnvironmentById(environmentId);

    //     if (environment == null)
    //         return NotFound("The environment with the specified ID does not exist.");

    //     if (environment.ownerUserId != loggedInUser)
    //         return Forbid("You do not have permission to view this environment.");

    //     return Ok(environment);
    // }


    [HttpPost(Name = "CreateEnvironment")]
    public async Task<ActionResult<Environment2D>> Post([FromBody] Environment2D newEnvironment)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        var existingEnvironments = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (existingEnvironments.Any(e => e.name == newEnvironment.name))
            return BadRequest("An environment with the same name already exists.");

        if (existingEnvironments.Count() >= 5)
            return BadRequest("You have reached the maximum number of environments allowed (5).");

        var environment = newEnvironment;
        environment.id = Guid.NewGuid();
        environment.ownerUserId = loggedInUser;

        Console.WriteLine($"Creating new environment: ID = {environment.id}, Name = {environment.name}, OwnerUserId = {environment.ownerUserId}, MaxHeight = {environment.maxHeight}, MaxLength = {environment.maxLength}");

        await _environmentRepository.CreateEnvironmentAsync(environment);

        return Ok(environment);
    }

    [HttpPut("{environmentId}", Name = "UpdateEnvironmentById")]
    public async Task<IActionResult> Put(Guid environmentId, Environment2D updatedEnvironment)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();
        var existingEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (existingEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (existingEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to update this environment.");

        updatedEnvironment.id = environmentId;
        updatedEnvironment.ownerUserId = loggedInUser;

        await _environmentRepository.UpdateEnvironmentByIdAsync(environmentId, updatedEnvironment);
        return Ok(updatedEnvironment);
    }


    [HttpDelete("{environmentId}", Name = "DeleteEnvironmentById")]
    public async Task<IActionResult> Delete(Guid environmentId)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();
        var existingEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (existingEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (existingEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to delete this environment.");

        await _environmentRepository.DeleteEnvironmentByIdAsync(environmentId);
        return NoContent();
    }


    /// OBJECTS

    // [HttpGet("{environmentId}/objects", Name = "GetObjectsByEnvironmentId")]
    // public async Task<IActionResult> GetObjects()
    // {
    //     await _objectRepository
    // }
}