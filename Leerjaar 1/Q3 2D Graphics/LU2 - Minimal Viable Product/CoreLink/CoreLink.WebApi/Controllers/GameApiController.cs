using CoreLink.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Identity;

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
    // TODO Refactor environment methodes to also check for empty Guid

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

        if (string.IsNullOrEmpty(newEnvironment.name) || newEnvironment.name.Length > 25)
            return BadRequest("The environment name must be between 1 and 25 characters.");

        if (newEnvironment.maxLength > 200 || newEnvironment.maxLength < 20)
            return BadRequest("The environment length must be between 20 and 200.");

        if (newEnvironment.maxHeight > 100 || newEnvironment.maxHeight < 10)
            return BadRequest("The environment height must be between 10 and 100.");


        var existingEnvironmentsForUser = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (existingEnvironmentsForUser.Any(e => e.name == newEnvironment.name))
            return BadRequest("An environment with the same name already exists.");

        if (existingEnvironmentsForUser.Count() >= 5)
            return BadRequest("You have reached the maximum number of environments allowed (5).");

        newEnvironment.id = Guid.NewGuid();
        newEnvironment.ownerUserId = loggedInUser;

        await _environmentRepository.CreateEnvironmentAsync(newEnvironment);

        return Ok(newEnvironment);
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

    [HttpGet("{environmentId}/objects", Name = "GetObjectsByEnvironmentId")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetObjects(Guid environmentId)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (environment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (environment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to view objects in this environment.");

        var objects = await _objectRepository.GetObject2DsAsync(environmentId);

        if (!objects.Any())
            return NotFound("No objects found in the specified environment.");

        return Ok(objects);
    }


    [HttpPost("{environmentId}/objects", Name = "CreateObject")]
    // TODO Maybe add Object2D to Task<ActionResult<Object2D>>
    public async Task<IActionResult> CreateObject(Guid environmentId, [FromBody] Object2D newObject)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (environment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (environment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to create objects in this environment.");

        // TODO Check constrains of the environment like rotation, scale and position

        newObject.id = new Guid();
        newObject.environmentId = environmentId;

        await _objectRepository.CreateObject(environmentId, newObject);

        return Ok(newObject);
    }


    [HttpPut("{environmentId}/objects/{objectId}", Name = "UpdateObject")]
    public async Task<IActionResult> UpdateObject(Guid environmentId, Guid objectId, [FromBody] Object2D updatedObject)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        if (objectId == Guid.Empty)
            return BadRequest("The Object ID is not valid.");

        var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (environment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (environment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to create objects in this environment.");

        updatedObject.id = objectId;
        // TODO Figure out if this is redundant code
        updatedObject.environmentId = environmentId;

        await _objectRepository.UpdateObject(objectId, updatedObject);
        return Ok(updatedObject);
    }

    [HttpDelete("{environmentId}/objects/{objectId}", Name = "DeleteObject")]
    public async Task<IActionResult> DeleteObject(Guid environmentId, Guid objectId)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        if (objectId == Guid.Empty)
            return BadRequest("The object ID is not valid.");

        var objectEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (objectEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (objectEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to delete objects in this environment.");

        var existingObject = await _objectRepository.GetObjectByIdAsync(objectId);

        if (existingObject == null)
            return NotFound("The object with the specified ID does not exist.");

        if (existingObject.environmentId != environmentId)
            return BadRequest("The object does not belong to the specified environment.");

        await _objectRepository.DeleteObject(objectId);
        return NoContent();
    }
}