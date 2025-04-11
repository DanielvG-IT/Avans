using HQB.WebApi.Controllers;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class AppointmentsControllerTest
    {
        public required Mock<IAppointmentRepository> _mockAppointmentRepository;
        public required Mock<IAuthenticationService> _mockAuthenticationService;
        public required Mock<ILogger<AppointmentsController>> _mockLogger;
        public required AppointmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockAppointmentRepository = new Mock<IAppointmentRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _controller = new AppointmentsController(
                _mockLogger.Object,
                _mockAppointmentRepository.Object,
                _mockAuthenticationService.Object
            );
        }

        [TestMethod]
        public async Task GetAppointmentsByTreatmentId_ValidTreatmentId_ReturnsOk()
        {
            // Arrange
            var treatmentId = Guid.NewGuid();
            var appointments = new List<Appointment>
            {
                new() { ID = Guid.NewGuid(), Name = "Test Appointment", Sequence = 1, Description = "Default Description" },
                new() { ID = Guid.NewGuid(), Name = "Another Appointment", Sequence = 2, Description = "Another Description" }
            };
            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns("user123");
            _mockAppointmentRepository.Setup(r => r.GetAppointmentsByTreatmentIdAsync(treatmentId))
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByTreatmentId(treatmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedAppointments = okResult.Value as List<Appointment>;
            Assert.IsNotNull(returnedAppointments);
            Assert.AreEqual(2, returnedAppointments.Count);
        }

        // TODO: Fix this test
        // [TestMethod]
        // public async Task GetAppointmentsByTreatmentId_InvalidTreatmentId_ReturnsBadRequest()
        // {
        //     // Act
        //     var result = await _controller.GetAppointmentsByTreatmentId(Guid.Empty);

        //     // Assert
        //     var badRequestResult = result.Result as BadRequestObjectResult;
        //     Assert.IsNotNull(badRequestResult);
        //     Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        // }

        [TestMethod]
        public async Task GetAppointmentById_ValidId_ReturnsOk()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Test Appointment", Description = "Test Description" };
            _mockAppointmentRepository.Setup(r => r.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);

            // Act
            var result = await _controller.GetAppointmentById(appointmentId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(appointment, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetAppointmentById(Guid.Empty);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateAppointment_ValidAppointment_ReturnsCreated()
        {
            // Arrange
            var appointment = new Appointment { Name = "Test Appointment", Description = "Test Description" };
            _mockAppointmentRepository.Setup(r => r.AddAppointmentAsync(It.IsAny<Appointment>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.CreateAppointment(appointment);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateAppointment_NullAppointment_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateAppointment(null!);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateAppointment_ValidAppointment_ReturnsNoContent()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Updated Appointment", Description = "Updated Description" };
            _mockAppointmentRepository.Setup(r => r.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);
            _mockAppointmentRepository.Setup(r => r.UpdateAppointmentAsync(appointment))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.UpdateAppointment(appointmentId, appointment);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteAppointment_ValidId_ReturnsNoContent()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment { ID = appointmentId, Name = "Default Name", Description = "Default Description" };
            _mockAppointmentRepository.Setup(r => r.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);
            _mockAppointmentRepository.Setup(r => r.DeleteAppointmentAsync(appointmentId))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteAppointment(appointmentId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
    }
}