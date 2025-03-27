using Moq;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class GuardianControllerTests
    {
        public required Mock<ILogger<GuardianController>> _mockLogger;
        public required Mock<IPatientRepository> _mockPatientRepo;
        public required Mock<IGuardianRepository> _mockRepo;
        public required Mock<IAuthenticationService> _mockAuthService;
        public required GuardianController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<GuardianController>>();
            _mockRepo = new Mock<IGuardianRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new GuardianController(_mockLogger.Object, _mockRepo.Object, _mockPatientRepo.Object, _mockAuthService.Object);
        }

        [TestMethod]
        public async Task GetGuardianForCurrentUser_ReturnsOkResult_WithGuardians()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", UserID = Guid.NewGuid().ToString() };
            var userId = Guid.NewGuid().ToString();
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockRepo.Setup(repo => repo.GetGuardianByUserIdAsync(userId)).ReturnsAsync(guardian);

            // Act
            var result = await _controller.GetGuardianForCurrentUser();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(guardian, okResult.Value);
        }

        [TestMethod]
        public async Task GetGuardianForCurrentUser_UserIdIsNull_ReturnsBadRequest()
        {
            // Arrange
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns<string?>(null!);

            // Act
            var result = await _controller.GetGuardianForCurrentUser();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Authenticated user ID is required.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetGuardianForCurrentUser_GuardiansAreNull_ReturnsInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockRepo.Setup(repo => repo.GetGuardianByUserIdAsync(userId)).ReturnsAsync((Guardian?)null);

            // Act
            var result = await _controller.GetGuardianForCurrentUser();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = result.Result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("Internal server error.", objectResult.Value);
        }

        [TestMethod]
        public async Task GetGuardianById_GuardianExists_ReturnsOk()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", UserID = Guid.NewGuid().ToString() };
            _mockRepo.Setup(repo => repo.GetGuardianByIdAsync(guardian.ID)).ReturnsAsync(guardian);

            // Act
            var result = await _controller.GetGuardianById(guardian.ID);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        }

        [TestMethod]
        public async Task GetGuardianById_GuardianNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetGuardianByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Guardian?)null);

            // Act
            var result = await _controller.GetGuardianById(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task PostGuardian_ValidGuardian_ReturnsCreated()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "First", LastName = "Last", UserID = Guid.NewGuid().ToString() };
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(Guid.NewGuid().ToString());
            _mockRepo.Setup(repo => repo.AddGuardianAsync(guardian)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddGuardian(guardian);

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
        }

        [TestMethod]
        public async Task PutGuardian_ValidGuardian_ReturnsOk()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Updated", LastName = "Guardian", UserID = Guid.NewGuid().ToString() };
            _mockRepo.Setup(repo => repo.UpdateGuardianAsync(guardian)).ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateGuardian(guardian.ID, guardian);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        }

        [TestMethod]
        public async Task DeleteGuardian_GuardianExists_ReturnsNoContent()
        {
            // Arrange
            var guardianId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.DeleteGuardianAsync(guardianId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteGuardian(guardianId);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }
    }
}
