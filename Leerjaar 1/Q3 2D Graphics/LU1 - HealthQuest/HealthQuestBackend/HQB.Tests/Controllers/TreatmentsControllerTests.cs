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
    public class TreatmentsControllerTest
    {
        public required Mock<ITreatmentRepository> _mockTreatmentRepository;
        public required Mock<IAppointmentRepository> _mockAppointmentRepository;
        public required Mock<ILogger<TreatmentsController>> _mockLogger;
        public required TreatmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockTreatmentRepository = new Mock<ITreatmentRepository>();
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<TreatmentsController>>();
            _controller = new TreatmentsController(
                _mockTreatmentRepository.Object,
                _mockAppointmentRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task GetTreatmentsAsync_ReturnsOk_WhenTreatmentsExist()
        {
            // Arrange
            var treatments = new List<Treatment>
            {
                new() { ID = Guid.NewGuid(), Name = "Treatment1" },
                new() { ID = Guid.NewGuid(), Name = "Treatment2" }
            };
            _mockTreatmentRepository.Setup(repo => repo.GetAllTreatmentsAsync()).ReturnsAsync(treatments);

            // Act
            var result = await _controller.GetTreatmentsAsync();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatments, okResult.Value);
        }

        [TestMethod]
        public async Task GetTreatmentsAsync_ReturnsNotFound_WhenNoTreatmentsExist()
        {
            // Arrange
            _mockTreatmentRepository.Setup(repo => repo.GetAllTreatmentsAsync()).ReturnsAsync([]);

            // Act
            var result = await _controller.GetTreatmentsAsync();

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetTreatmentByIdAsync_ReturnsOk_WhenTreatmentExists()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Treatment1" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);

            // Act
            var result = await _controller.GetTreatmentByIdAsync(treatmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatment, okResult.Value);
        }

        [TestMethod]
        public async Task GetTreatmentByIdAsync_ReturnsNotFound_WhenTreatmentDoesNotExist()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync((Treatment)null!);

            // Act
            var result = await _controller.GetTreatmentByIdAsync(treatmentId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateTreatmentAsync_ReturnsCreatedAtRoute_WhenTreatmentIsCreated()
        {
            // Arrange
            var treatment = new Treatment { Name = "New Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.AddTreatmentAsync(It.IsAny<Treatment>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateTreatmentAsync(treatment);

            // Assert
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            Assert.IsNotNull(createdAtRouteResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtRouteResult.StatusCode);
            Assert.AreEqual("GetTreatmentById", createdAtRouteResult.RouteName);
        }

        [TestMethod]
        public async Task UpdateTreatmentAsync_ReturnsOk_WhenTreatmentIsUpdated()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Updated Treatment" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);
            _mockTreatmentRepository.Setup(repo => repo.UpdateTreatmentAsync(treatment)).ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateTreatmentAsync(treatmentId, treatment);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(treatment, okResult.Value);
        }

        [TestMethod]
        public async Task DeleteTreatmentAsync_ReturnsNoContent_WhenTreatmentIsDeleted()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var treatment = new Treatment { ID = treatmentId, Name = "Treatment to Delete" };
            _mockTreatmentRepository.Setup(repo => repo.GetTreatmentByIdAsync(treatmentId)).ReturnsAsync(treatment);
            _mockTreatmentRepository.Setup(repo => repo.DeleteTreatmentAsync(treatmentId)).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.DeleteTreatmentAsync(treatmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
    }
}