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
        public required Mock<IAuthenticationService> _mockAuthenticationService;
        public required Mock<IAppointmentRepository> _mockAppointmentRepository;
        public required AppointmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _controller = new AppointmentsController(_mockLogger.Object, _mockAppointmentRepository.Object, _mockAuthenticationService.Object);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_NoAuthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns(string.Empty);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(null);

            // Assert
            Assert.IsInstanceOfType<UnauthorizedObjectResult>(result.Result);
            Assert.IsNotNull(result.Result, "Expected UnauthorizedObjectResult but got null.");
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult, "Expected UnauthorizedObjectResult but got null.");
            Assert.AreEqual("No authenticated user found", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_NoTreatmentId_ReturnsAllAppointments()
        {
            // Arrange
            var mockAppointments = new List<Appointment>
            {
                new Appointment { ID = Guid.NewGuid(), Name = "Appointment 1", Description = "Description for Appointment 1" },
                new Appointment { ID = Guid.NewGuid(), Name = "Appointment 2", Description = "Description for Appointment 2" }
            };

            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockAppointmentRepository.Setup(r => r.GetAllAppointmentsAsync()).ReturnsAsync(mockAppointments);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(null);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
            var appointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(appointments, "Expected appointments to be not null.");
            Assert.AreEqual(2, appointments.Count());
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ValidTreatmentId_ReturnsAppointments()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var mockTreatmentAppointments = new List<TreatmentAppointment>
            {
                new() { AppointmentID = Guid.NewGuid(), TreatmentID = treatmentId, Sequence = 1 },
                new() { AppointmentID = Guid.NewGuid(), TreatmentID = treatmentId, Sequence = 2 }
            };

            var mockAppointments = new List<Appointment>
            {
                new() { ID = mockTreatmentAppointments[0].AppointmentID, Name = "Appointment 1", Description = "Description for Appointment 1" },
                new() { ID = mockTreatmentAppointments[1].AppointmentID, Name = "Appointment 2", Description = "Description for Appointment 2" }
            };

            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockAppointmentRepository.Setup(r => r.GetAppointmentsByTreatmentIdAsync(treatmentId)).ReturnsAsync(mockTreatmentAppointments.AsEnumerable());
            _mockAppointmentRepository.Setup(r => r.GetAppointmentByIdAsync(mockTreatmentAppointments[0].AppointmentID)).ReturnsAsync(mockAppointments[0]);
            _mockAppointmentRepository.Setup(r => r.GetAppointmentByIdAsync(mockTreatmentAppointments[1].AppointmentID)).ReturnsAsync(mockAppointments[1]);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(treatmentId);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
            var appointments = okResult.Value as IEnumerable<Appointment>;
            Assert.IsNotNull(appointments, "Expected appointments to be not null.");
            Assert.AreEqual(2, appointments.Count());
            Assert.AreEqual("Appointment 1", appointments.First().Name);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_NoValidAppointments_ReturnsNotFound()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();

            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockAppointmentRepository.Setup(r => r.GetAppointmentsByTreatmentIdAsync(treatmentId)).ReturnsAsync(new List<TreatmentAppointment>
            {
                new() { AppointmentID = Guid.Empty, TreatmentID = treatmentId, Sequence = 0 }
            });

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(treatmentId);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(result.Result);
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult but got null.");
            Assert.AreEqual($"No valid appointments found for the treatment ID: {treatmentId}", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsOkResult_WithAppointment()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment", Description = "Test Description" };
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            var result = await _controller.GetAppointmentById(appointmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(appointment, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsBadRequest_WhenIdIsEmpty()
        {
            // Arrange
            var emptyId = Guid.Empty;

            // Act
            var result = await _controller.GetAppointmentById(emptyId);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null.");
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("The provided appointment ID is invalid. Please provide a valid ID.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task CreateAppointment_ReturnsCreatedAtActionResult_WithNewAppointment()
        {
            // Arrange
            var appointment = new Appointment { ID = Guid.NewGuid(), Name = "New Appointment", Description = "Description for New Appointment" };
            _mockAppointmentRepository.Setup(repo => repo.AddAppointmentAsync(It.IsAny<Appointment>())).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateAppointment(appointment);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(appointment, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task UpdateAppointment_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Updated Appointment", Description = "Updated Description" };
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);
            _mockAppointmentRepository.Setup(repo => repo.UpdateAppointmentAsync(appointment)).Returns(Task.FromResult(1));

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
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment", Description = "Test Description" };
            _mockAppointmentRepository.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);
            _mockAppointmentRepository.Setup(repo => repo.DeleteAppointmentAsync(appointmentId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteAppointment(appointmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}