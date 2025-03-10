using Moq;
using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Controllers;

namespace CoreLink.Tests
{
    [TestClass]
    public class GameApiControllerTests
    {
        private Mock<IEnvironmentRepository>? _mockEnvironmentRepo;
        private Mock<IAuthenticationService>? _mockAuthService;
        private Mock<IObjectRepository>? _mockObjectRepo;
        private GameApiController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockEnvironmentRepo = new Mock<IEnvironmentRepository>();
            _mockObjectRepo = new Mock<IObjectRepository>();
            _mockAuthService = new Mock<IAuthenticationService>();

            _controller = new GameApiController(
                _mockAuthService.Object,
                _mockEnvironmentRepo.Object,
                _mockObjectRepo.Object
            );
        }

        // Test GET: /environments (Get all environments)
        // [TestMethod]
        // public async Task Get_ShouldReturnOk_WhenUserIsAuthenticated()
        // {
        //     // Arrange
        //     var userId = "user123";
        //     var environments = new List<Environment2D>
        //     {
        //         new() { id = Guid.NewGuid(), name = "Test Environment", ownerUserId = userId }
        //     };

        //     _mockAuthService!.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(userId);
        //     _mockEnvironmentRepo!.Setup(repo => repo.GetEnvironmentsByUserIdAsync(userId)).ReturnsAsync(environments);

        //     // Act
        //     var result = await _controller!.Get();

        //     // Assert
        //     var okResult = result;
        //     Assert.IsNotNull(okResult);
        //     var returnedEnvironments = okResult.Value; // No enviroments are returned because user123 is not the owner of any environments
        //     Assert.IsNotNull(returnedEnvironments);

        //     Assert.AreEqual(1, returnedEnvironments.Count());
        // }

        // Test POST: /environments (Add environment)
        [TestMethod]
        public async Task Post_ShouldReturnBadRequest_WhenEnvironmentNameIsInvalid()
        {
            // Arrange
            var newEnvironment = new Environment2D { name = "Invalid Environment Name that is too long", maxLength = 100, maxHeight = 50 };
            _mockAuthService!.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns("user123");

            // Act
            var result = await _controller!.Post(newEnvironment);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("The environment name must be between 1 and 25 characters.", badRequestResult.Value);
        }

        // Test PUT: /environments/{id} (Edit environment)
        [TestMethod]
        public async Task Put_ShouldReturnNotFound_WhenEnvironmentDoesNotExist()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var updatedEnvironment = new Environment2D { id = environmentId, name = "Updated Environment", maxHeight = 60, maxLength = 150 };

            _mockAuthService!.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockEnvironmentRepo!.Setup(repo => repo.GetEnvironmentByIdAsync(environmentId)).ReturnsAsync((Environment2D?)null);

            // Act
            var result = await _controller!.Put(environmentId, updatedEnvironment);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("The environment with the specified ID does not exist.", notFoundResult.Value);
        }

        // Test DELETE: /environments/{id} (Remove environment)
        [TestMethod]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessfullyDeleted()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var environment = new Environment2D { id = environmentId, name = "Test Environment", ownerUserId = "user123" };

            _mockAuthService!.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockEnvironmentRepo!.Setup(repo => repo.GetEnvironmentByIdAsync(environmentId)).ReturnsAsync(environment);

            // Act
            var result = await _controller!.Delete(environmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
        }

        // Test GET: /environments/{environmentId}/objects (Get objects of environment)
        [TestMethod]
        public async Task GetObjects_ShouldReturnOk_WhenObjectsAreFound()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var userId = "user123";
            var objects = new List<Object2D>
            {
                new() { id = Guid.NewGuid(), environmentId = environmentId, prefabId = "blueChip" }
            };

            _mockAuthService!.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockEnvironmentRepo!.Setup(repo => repo.GetEnvironmentByIdAsync(environmentId)).ReturnsAsync(new Environment2D { id = environmentId, ownerUserId = userId });
            _mockObjectRepo!.Setup(repo => repo.GetObjectsByEnvironmentIdAsync(environmentId)).ReturnsAsync(objects);

            // Act
            var result = await _controller!.GetObjects(environmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedObjects = okResult.Value as IEnumerable<Object2D>;
            Assert.IsNotNull(returnedObjects);
            Assert.AreEqual(1, returnedObjects.Count());
        }
    }
}
