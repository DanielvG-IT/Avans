using Moq;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class GuardianControllerTests
    {
        public required Mock<IGuardianRepository> _mockRepo;
        public required Mock<ILogger<GuardianController>> _mockLogger;
        public required GuardianController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<GuardianController>>();
            _controller = new GuardianController(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetOuderVoogd_ReturnsOkResult_WithGuardians()
        {
            // Arrange
            var guardians = new List<Guardian> { new() { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", UserID = Guid.NewGuid().ToString() } };
            _mockRepo.Setup(repo => repo.GetAllGuardiansAsync()).ReturnsAsync(guardians);

            // Act
            var result = await _controller.GetOuderVoogd();

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(guardians, okResult.Value);
        }

        [TestMethod]
        public async Task GetOuderVoogdById_GuardianExists_ReturnsOk()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", UserID = Guid.NewGuid().ToString() };
            _mockRepo.Setup(repo => repo.GetGuardianByIdAsync(guardian.ID)).ReturnsAsync(guardian);

            // Act
            var result = await _controller.GetOuderVoogdById(guardian.ID);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result);
        }

        [TestMethod]
        public async Task GetOuderVoogdById_GuardianNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetGuardianByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Guardian?)null);

            // Act
            var result = await _controller.GetOuderVoogdById(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task PostOuderVoogd_ValidGuardian_ReturnsCreated()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "First", LastName = "Last", UserID = Guid.NewGuid().ToString() };
            _mockRepo.Setup(repo => repo.AddGuardianAsync(guardian)).ReturnsAsync(1);

            // Act
            var result = await _controller.PostOuderVoogd(guardian);

            // Assert
            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
        }

        [TestMethod]
        public async Task PutOuderVoogd_ValidGuardian_ReturnsNoContent()
        {
            // Arrange
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Updated", LastName = "Guardian", UserID = Guid.NewGuid().ToString() };
            _mockRepo.Setup(repo => repo.UpdateGuardianAsync(guardian)).ReturnsAsync(1);

            // Act
            var result = await _controller.PutOuderVoogd(guardian.ID, guardian);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }

        [TestMethod]
        public async Task DeleteOuderVoogd_GuardianExists_ReturnsNoContent()
        {
            // Arrange
            var guardianId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.DeleteGuardianAsync(guardianId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteOuderVoogd(guardianId);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }
    }
}
