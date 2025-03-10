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
        _authenticationService = authenticationService;
        _environmentRepository = environmentRepository;
        _objectRepository = objectRepository;
    }

    // ENVIRONMENT METHODS
    [HttpGet(Name = "GetUserEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        var environments = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (environments == null)
            return StatusCode(500, "Error retrieving environments from the repository.");

        if (!environments.Any())
            return NotFound("No environments found for the current user.");

        if (environments.Count() > 5)
            return BadRequest("Too many environments found for the current user.");

        return Ok(environments);
    }

    [HttpPost(Name = "AddEnvironment")]
    public async Task<ActionResult<Environment2D>> Post([FromBody] Environment2D newEnvironment)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (string.IsNullOrEmpty(newEnvironment.name) || newEnvironment.name.Length > 25)
            return BadRequest("The environment name must be between 1 and 25 characters.");

        if (newEnvironment.maxLength < 20 || newEnvironment.maxLength > 200)
            return BadRequest("The environment length must be between 20 and 200.");

        if (newEnvironment.maxHeight < 10 || newEnvironment.maxHeight > 100)
            return BadRequest("The environment height must be between 10 and 100.");

        var existingEnvironmentsForUser = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (existingEnvironmentsForUser.Any(e => e.name == newEnvironment.name))
            return BadRequest("An environment with the same name already exists.");

        if (existingEnvironmentsForUser.Count() >= 5)
            return BadRequest("You have reached the maximum number of environments allowed (5).");

        newEnvironment.id = Guid.NewGuid();
        newEnvironment.ownerUserId = loggedInUser;

        if (newEnvironment.id == Guid.Empty)
            return StatusCode(500, "Failed to generate a valid GUID for the new environment.");

        await _environmentRepository.CreateEnvironmentAsync(newEnvironment);

        return Ok(newEnvironment);
    }

    [HttpPut("{environmentId}", Name = "EditEnvironment")]
    public async Task<ActionResult> Put(Guid environmentId, [FromBody] Environment2D updatedEnvironment)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        var existingEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (existingEnvironment.name == updatedEnvironment.name)
            return BadRequest("An environment with the same name already exists.");
        
        var existingEnvironmentsForUser = await _environmentRepository.GetEnvironmentsByUserIdAsync(loggedInUser);

        if (existingEnvironmentsForUser.Any(e => e.name == updatedEnvironment.name))
            return BadRequest("An environment with the same name already exists.");

        if (existingEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to update this environment.");

        var objectsInEnvironment = await _objectRepository.GetObjectsByEnvironmentIdAsync(existingEnvironment.id);

        foreach (var obj in objectsInEnvironment)
        {
            if (!existingEnvironment.IsPositionValid(obj.positionX, obj.positionY))
                return BadRequest("One or more objects have invalid positions.");
        }

        updatedEnvironment.id = environmentId;
        updatedEnvironment.ownerUserId = loggedInUser;

        await _environmentRepository.UpdateEnvironmentByIdAsync(environmentId, updatedEnvironment);

        return Ok(updatedEnvironment);
    }

    [HttpDelete("{environmentId}", Name = "RemoveEnvironment")]
    public async Task<ActionResult> Delete(Guid environmentId)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty)
            return BadRequest("The environment ID is not valid.");

        var existingEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);

        if (existingEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (existingEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to delete this environment.");

        await _objectRepository.DeleteObjectsByEnvironmentIdAsync(environmentId);
        await _environmentRepository.DeleteEnvironmentByIdAsync(environmentId);

        return NoContent();
    }

    // OBJECT METHODS
    [HttpGet("{environmentId}/objects", Name = "GetEnvironmentObjects")]
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

        var objects = await _objectRepository.GetObjectsByEnvironmentIdAsync(environmentId);

        if (!objects.Any())
            return NotFound("No objects found in the specified environment.");

        return Ok(objects);
    }

    [HttpPost("{environmentId}/objects", Name = "AddEnvironmentObject")]
    public async Task<ActionResult<Object2D>> CreateObject(Guid environmentId, [FromBody] Object2D newObject)
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

        bool isPositionValid = environment.IsPositionValid(newObject.positionX, newObject.positionY);

        if (!isPositionValid)
            return BadRequest("The position of the object is not valid within the environment.");

        newObject.id = Guid.NewGuid();
        newObject.environmentId = environmentId;

        await _objectRepository.CreateObject(environmentId, newObject);

        return CreatedAtRoute("GetEnvironmentObjects", new { environmentId, objectId = newObject.id }, newObject);
    }

    [HttpPut("{environmentId}/objects/{objectId}", Name = "EditEnvironmentObject")]
    public async Task<ActionResult> UpdateObject(Guid environmentId, Guid objectId, [FromBody] Object2D updatedObject)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty || objectId == Guid.Empty)
            return BadRequest("The environment or object ID is not valid.");

        var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);
        if (environment == null)
            return NotFound("The environment with the specified ID does not exist.");

        var existingObject = await _objectRepository.GetObjectByIdAsync(objectId);
        if (existingObject == null || existingObject.environmentId != environmentId)
            return NotFound("The object with the specified ID does not exist or does not belong to this environment.");

        if (environment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to update this object.");

        updatedObject.id = objectId;
        updatedObject.environmentId = environmentId;

        await _objectRepository.UpdateObjectByIdAsync(objectId, updatedObject);

        return Ok(updatedObject);
    }

    [HttpDelete("{environmentId}/objects/{objectId}", Name = "RemoveEnvironmentObject")]
    public async Task<ActionResult> DeleteObject(Guid environmentId, Guid objectId)
    {
        var loggedInUser = _authenticationService.GetCurrentAuthenticatedUserId();

        if (string.IsNullOrEmpty(loggedInUser))
            return Unauthorized("User is not authenticated.");

        if (environmentId == Guid.Empty || objectId == Guid.Empty)
            return BadRequest("The environment or object ID is not valid.");

        var objectEnvironment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId);
        if (objectEnvironment == null)
            return NotFound("The environment with the specified ID does not exist.");

        if (objectEnvironment.ownerUserId != loggedInUser)
            return Forbid("You do not have permission to delete objects in this environment.");

        var existingObject = await _objectRepository.GetObjectByIdAsync(objectId);
        if (existingObject == null || existingObject.environmentId != environmentId)
            return NotFound("The object with the specified ID does not exist or does not belong to this environment.");

        await _objectRepository.DeleteObjectByIdAsync(objectId);

        return NoContent();
    }
}
