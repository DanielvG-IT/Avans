using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreLink.WebApi.Repositories
{
    public static class EnvironmentRepository
    {
        public static List<Environment2D> Environments { get; } = [];

        // Add methodes
        public static int AddEnvironment(Environment2D environment)
        {
            // Check if an environment with the same name already exists
            if (Environments.Any(e => e.name == environment.name))
            {
                return StatusCodes.Status400BadRequest; // Return 400 if the name already exists
            }

            Environments.Add(environment);
            return StatusCodes.Status201Created; // Return 201 if successfully added
        }

        // Update methodes
        public static IActionResult UpdateEnvironment(string existingName, Environment2D newEnvironment)
        {
            var existingEnvironment = Environments.FirstOrDefault(e => e.name == existingName);

            if (existingEnvironment == null)
            {
                return new NotFoundObjectResult("Environment not found.");
            }

            Environments.Remove(existingEnvironment);
            Environments.Add(newEnvironment);

            return new OkObjectResult(newEnvironment);

        }

        // Delete methodes
        public static int RemoveEnvironment(string nameDeleting)
        {
            var wantingToDelete = Environments.FirstOrDefault(e => e.name == nameDeleting);
            
            if (wantingToDelete == null) 
            { 
                return StatusCodes.Status404NotFound; 
            }

            Environments.Remove(wantingToDelete);

            return StatusCodes.Status204NoContent;
        }


        // Get methodes
        public static List<Environment2D> GetEnvironments()
        {
            return Environments;
        }

        public static Environment2D? GetEnvironment(string name)
        {
            return Environments.FirstOrDefault(e => e.name == name);
        }
    }
}
