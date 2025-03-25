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
    public class TreatmentsControllerTests
    {
        public required Mock<IAppointmentRepository> _mockAppointmentRepository;
        public required Mock<ITreatmentRepository> _mockTreatmentRepository;
        public required Mock<ILogger<TreatmentsController>> _mockLogger;
        public required TreatmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockTreatmentRepository = new Mock<ITreatmentRepository>();
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<TreatmentsController>>();
            _controller = new TreatmentsController(_mockTreatmentRepository.Object, _mockAppointmentRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetTreatmentsAsync_ReturnsOkResult_WithListOfTreatments()
        {
            // Arrange
            var treatments = new List<Treatment> { new Treatment { ID = Guid.NewGuid(), Name = "Test Treatment" } };
            _mockTreatmentRepository.Setup(repo => repo.GetAllTreatmentsAsync()).ReturnsAsync(treatments);

            // Act
            var result = await _controller.GetTreatmentsAsync();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatments, okResult.Value);
        }

        [TestMethod]
        public async Task GetTreatmentAsync_ReturnsNotFound_WhenTreatmentDoesNotExist()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync((Treatment?)null);

            // Act
            var result = await _controller.GetTreatmentAsync(treatmentId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetTreatmentAsync_ReturnsOkResult_WithTreatment()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Test Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);

            // Act
            var result = await _controller.GetTreatmentAsync(treatmentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatment, okResult.Value);
        }

        [TestMethod]
        public async Task CreateTreatmentAsync_ReturnsBadRequest_WhenTreatmentIsNull()
        {
            // Act
            var result = await _controller.CreateTreatmentAsync(null!);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateTreatmentAsync_ReturnsCreatedAtActionResult_WithNewTreatment()
        {
            // Arrange
            var treatment = new Treatment { Name = "New Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.AddTreatmentAsync(It.IsAny<Treatment>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateTreatmentAsync(treatment);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            var returnedTreatment = createdAtActionResult.Value as Treatment;
            Assert.IsNotNull(returnedTreatment);
            Assert.AreEqual(treatment.Name, returnedTreatment.Name);
        }

        [TestMethod]
        public async Task UpdateTreatmentAsync_ReturnsNotFound_WhenTreatmentDoesNotExist()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Updated Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync((Treatment?)null);

            // Act
            var result = await _controller.UpdateTreatmentAsync(treatmentId, treatment);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateTreatmentAsync_ReturnsOkResult_WithUpdatedTreatment()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Updated Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);
            _mockTreatmentRepository.Setup(repo => repo.UpdateTreatmentAsync(treatment)).ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateTreatmentAsync(treatmentId, treatment);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatment, okResult.Value);
        }

        [TestMethod]
        public async Task DeleteTreatmentAsync_ReturnsNotFound_WhenTreatmentDoesNotExist()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync((Treatment?)null);

            // Act
            var result = await _controller.DeleteTreatmentAsync(treatmentId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteTreatmentAsync_ReturnsNoContent_WhenTreatmentIsDeleted()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Test Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);
            _mockTreatmentRepository.Setup(repo => repo.DeleteTreatmentAsync(treatmentId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteTreatmentAsync(treatmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
    }
}