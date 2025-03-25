using Moq;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        public required Mock<ILogger<AppointmentsController>> _mockLogger;
        public required Mock<IAppointmentRepository> _mockRepo;
        public required AppointmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IAppointmentRepository>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _controller = new AppointmentsController(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAppointments_ReturnsOkResult_WithListOfAppointments()
        {
            // Arrange
            var appointments = new List<Appointment> { new() { ID = Guid.NewGuid(), Name = "Test Appointment" } };
            _mockRepo.Setup(repo => repo.GetAllAppointmentsAsync()).ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointments();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(appointments, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsOkResult_WithAppointment()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment" };
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            var result = await _controller.GetAppointmentById(appointmentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(appointment, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            // Act
            var result = await _controller.GetAppointmentById(appointmentId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateAppointment_ReturnsCreatedAtActionResult_WithNewAppointment()
        {
            // Arrange
            var appointment = new Appointment { ID = Guid.NewGuid(), Name = "New Appointment" };
            _mockRepo.Setup(repo => repo.AddAppointmentAsync(It.IsAny<Appointment>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateAppointment(appointment);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(appointment, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task UpdateAppointment_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Updated Appointment" };
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);
            _mockRepo.Setup(repo => repo.UpdateAppointmentAsync(appointment)).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.UpdateAppointment(appointmentId, appointment);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAppointment_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment" };
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);
            _mockRepo.Setup(repo => repo.DeleteAppointmentAsync(appointmentId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteAppointment(appointmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}