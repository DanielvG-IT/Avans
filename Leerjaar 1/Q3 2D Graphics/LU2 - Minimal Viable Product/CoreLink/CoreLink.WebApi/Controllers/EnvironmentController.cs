using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreLink.WebApi.Controllers;

[ApiController]
[Route("environments")]
public class EnvironmentsController(IEnvironmentRepository environmentRepository, IObjectRepository objectRepository) : ControllerBase
{
    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> Get()
    {
        var environments = await environmentRepository.GetAllEnvironments();

        if (!environments.Any())
        {
            return NotFound();
        }

        return Ok(environments);
    }

    [HttpGet("{environmentId}", Name = "ReadEnvironmentById")]
    public async Task<ActionResult<Environment2D>> Get(Guid environmentId)
    {
        var environment = await environmentRepository.GetEnvironmentById(environmentId);

        if (environment == null)
        {
            return NotFound();
        }

        return Ok(environment);
    }

    // [HttpGet("{UserId}", Name = "ReadEnvironmentsByUserId")]
    // public async Task<ActionResult<IEnumerable<Environment2D>>> Get(Guid UserId)
    // {
    //     var environments = await environmentRepository.GetEnvironmentByUserId(UserId);

    //     if (!environments.Any())
    //     {
    //         return NotFound();
    //     }

    //     return Ok(environments);
    // }

    [HttpPost(Name = "CreateEnvironment")]
    public async Task<IActionResult> Post(Environment2D newEnvironment)
    {
        newEnvironment.Id = Guid.NewGuid();
        await environmentRepository.CreateEnvironment(newEnvironment);

        return CreatedAtRoute("ReadEnvironments", newEnvironment);
    }

    // [HttpPost(Name = "CreateEnvironment")]
    // public async Task<IActionResult> Post(Environment2D newEnvironment)
    // {
    //     // TODO Check if the user is allowed to create an environment
    //     // TODO Check if environment with same name already exists
    //     newEnvironment.Id = Guid.NewGuid();
    //     await environmentRepository.CreateEnvironment(newEnvironment);

    //     return CreatedAtRoute("ReadEnvironmentsByUserId", new { UserId = newEnvironment.UserId }, newEnvironment);
    // }

    [HttpPut("{Id}", Name = "UpdateEnvironmentByGuid")]
    public async Task<IActionResult> Put(Guid Id, Environment2D updatedEnvironment)
    {
        // TODO Check if environment is the users own environment
        var environment = await environmentRepository.GetEnvironmentById(Id);
        if (environment == null) { return NotFound(); }

        await environmentRepository.UpdateEnvironmentById(Id, updatedEnvironment);

        return Ok();
    }

    [HttpDelete("{Id}", Name = "DeleteEnvironmentByGuid")]
    public async Task<IActionResult> Delete(Guid Id)
    {
        // TODO Check if environment is users own environment
        var environment = await environmentRepository.GetEnvironmentById(Id);
        if (environment == null) { return NotFound(); }

        await environmentRepository.DeleteEnvironmentById(Id);

        return NoContent();
    }















    [HttpGet(Name = "ReadEnvironments")]
    public async Task<ActionResult<IEnumerable<Environment2D>>> GetObjects()
    {
        var environments = await environmentRepository.GetAllEnvironments();

        if (!environments.Any())
        {
            return NotFound();
        }

        return Ok(environments);
    }

    [HttpGet("{environmentId}/objects", Name = "ReadObjectsByEnvironmentId")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetObjects(Guid environmentId)
    {
        var objects = await objectRepository.GetObjectsByEnvironmentId(environmentId);

        if (objects == null)
        {
            return NotFound();
        }

        return Ok(objects);
    }

    [HttpPost("{id}/objects", Name = "CreateObject")]
    public async Task<IActionResult> PostObject(Guid id, Object2D object2d)
    {
        object2d.Id = Guid.NewGuid();
        await objectRepository.CreateObject(id, object2d);

        return CreatedAtRoute("ReadObjectsByEnvironmentId", object2d);
    }

    // [HttpPost(Name = "CreateEnvironment")]
    // public async Task<IActionResult> Post(Environment2D newEnvironment)
    // {
    //     // TODO Check if the user is allowed to create an environment
    //     // TODO Check if environment with same name already exists
    //     newEnvironment.Id = Guid.NewGuid();
    //     await environmentRepository.CreateEnvironment(newEnvironment);

    //     return CreatedAtRoute("ReadEnvironmentsByUserId", new { UserId = newEnvironment.UserId }, newEnvironment);
    // }

    [HttpPut("{Id}/objects", Name = "UpdateObjectByGuid")]
    public async Task<IActionResult> PutObject(Guid Id, Object2D updatedObject)
    {
        // TODO Check if environment is the users own environment
        var environment = await environmentRepository.GetEnvironmentById(Id);
        if (environment == null) { return NotFound(); }

        await objectRepository.UpdateObjectById(Id, updatedObject);

        return Ok();
    }

    [HttpDelete("{Id}/objects", Name = "DeleteObjectByGuid")]
    public async Task<IActionResult> DeleteObject(Guid Id)
    {
        // TODO Check if environment is users own environment
        var environment = await objectRepository.GetObjectById(Id);
        if (environment == null) { return NotFound(); }

        await objectRepository.DeleteObjectById(Id);

        return NoContent();
    }
}
