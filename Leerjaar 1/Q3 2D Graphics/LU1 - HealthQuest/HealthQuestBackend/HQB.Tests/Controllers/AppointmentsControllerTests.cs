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
        public required Mock<IAuthenticationService> _mockAuthService;
        public required Mock<IAppointmentRepository> _mockRepo;
        public required AppointmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IAppointmentRepository>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _controller = new AppointmentsController(_mockLogger.Object, _mockRepo.Object, _mockAuthService.Object);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ReturnsOkResult_WithSortedAppointments()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var appointment1 = new Appointment { ID = Guid.NewGuid(), Name = "Appointment 1", Description = "Description for Appointment 1" };
            var appointment2 = new Appointment { ID = Guid.NewGuid(), Name = "Appointment 2", Description = "Description for Appointment 2" };
            var treatmentAppointments = new List<(Guid AppointmentID, int Sequence)>
            {
            (appointment1.ID, 2),
            (appointment2.ID, 1)
            };

            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockRepo.Setup(repo => repo.GetAppointmentsByTreatmentIdAsync(treatmentId)).ReturnsAsync(treatmentAppointments.Select(t => new TreatmentAppointment { AppointmentID = t.AppointmentID, Sequence = t.Sequence }));
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointment1.ID)).ReturnsAsync(appointment1);
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointment2.ID)).ReturnsAsync(appointment2);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(treatmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var sortedAppointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(sortedAppointments);
            Assert.AreEqual(2, sortedAppointments.Count);
            Assert.AreEqual(appointment2, sortedAppointments[0]); // Sequence 1
            Assert.AreEqual(appointment1, sortedAppointments[1]); // Sequence 2
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(string.Empty);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(Guid.NewGuid());

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("No authenticated user found", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ReturnsNotFound_WhenNoAppointmentsExist()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockRepo.Setup(repo => repo.GetAppointmentsByTreatmentIdAsync(treatmentId)).ReturnsAsync(new List<TreatmentAppointment>());

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(treatmentId);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            Assert.AreEqual($"No appointments found for the treatment ID: {treatmentId}", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ReturnsOkResult_WithAllAppointments_WhenTreatmentIdIsNull()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
            new() { ID = Guid.NewGuid(), Name = "Appointment 1", Description = "Description for Appointment 1" },
            new() { ID = Guid.NewGuid(), Name = "Appointment 2", Description = "Description for Appointment 2" }
            };

            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockRepo.Setup(repo => repo.GetAllAppointmentsAsync()).ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(appointments, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsOkResult_WithAppointment()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment", Description = "Test Description" };
            _mockRepo.Setup(repo => repo.GetAppointmentByIdAsync(appointmentId)).ReturnsAsync(appointment);

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
            _mockRepo.Setup(repo => repo.AddAppointmentAsync(It.IsAny<Appointment>())).ReturnsAsync(1);

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
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment", Description = "Test Description" };
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