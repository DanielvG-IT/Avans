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

namespace HQB.WebApi.Tests.Controllers
{
    [TestClass]
    public class PatientControllerTest
    {
        public required Mock<ILogger<PatientController>> _loggerMock;
        public required Mock<IDoctorRepository> _doctorRepositoryMock;
        public required Mock<IPatientRepository> _patientRepositoryMock;
        public required Mock<IJournalRepository> _journalRepositoryMock;
        public required Mock<IStickersRepository> _stickersRepositoryMock;
        public required Mock<IGuardianRepository> _guardianRepositoryMock;
        public required Mock<ITreatmentRepository> _treatmentRepositoryMock;
        public required Mock<IAppointmentRepository> _appointmentRepositoryMock;
        public required Mock<IAuthenticationService> _authenticationServiceMock;
        public required Mock<IPersonalAppointmentsRepository> _personalAppointmentsRepositoryMock;
        public required PatientController _controller;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<PatientController>>();
            _doctorRepositoryMock = new Mock<IDoctorRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _journalRepositoryMock = new Mock<IJournalRepository>();
            _stickersRepositoryMock = new Mock<IStickersRepository>();
            _guardianRepositoryMock = new Mock<IGuardianRepository>();
            _treatmentRepositoryMock = new Mock<ITreatmentRepository>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _personalAppointmentsRepositoryMock = new Mock<IPersonalAppointmentsRepository>();

            _controller = new PatientController(
                _loggerMock.Object,
                _doctorRepositoryMock.Object,
                _patientRepositoryMock.Object,
                _journalRepositoryMock.Object,
                _stickersRepositoryMock.Object,
                _guardianRepositoryMock.Object,
                _treatmentRepositoryMock.Object,
                _appointmentRepositoryMock.Object,
                _authenticationServiceMock.Object,
                _personalAppointmentsRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task GetPatientsForCurrentUser_ReturnsOkResult_WithPatients()
        {
            // Arrange
            var userId = "test-user-id";
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" };
            var patients = new List<Patient>
            {
                new() { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "DefaultAvatar" }
            };

            _authenticationServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
            _guardianRepositoryMock.Setup(g => g.GetGuardianByUserIdAsync(userId)).ReturnsAsync(guardian);
            _patientRepositoryMock.Setup(p => p.GetPatientsByGuardianId(guardian.ID)).ReturnsAsync(patients);

            // Act
            var result = await _controller.GetPatientsForCurrentUser();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(patients, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatientsForCurrentUser_ReturnsBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            _authenticationServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns((string)null!);

            // Act
            var result = await _controller.GetPatientsForCurrentUser();

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPatientById_ReturnsOkResult_WithPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new Patient { ID = patientId, FirstName = "John", LastName = "Doe", Avatar = "DefaultAvatar" };

            _patientRepositoryMock.Setup(p => p.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(patient, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _patientRepositoryMock.Setup(p => p.GetPatientByIdAsync(patientId)).ReturnsAsync((Patient)null!);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // TODO: Fix this test
        // [TestMethod]
        // public async Task AddPatient_ReturnsCreatedResult_WithPatient()
        // {
        //     // Arrange
        //     var userId = "test-user-id";
        //     var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" };
        //     var patient = new Patient { FirstName = "John", LastName = "Doe", Avatar = "DefaultAvatar" };

        //     _authenticationServiceMock.Setup(a => a.GetCurrentAuthenticatedUserId()).Returns(userId);
        //     _guardianRepositoryMock.Setup(g => g.GetGuardianByUserIdAsync(userId)).ReturnsAsync(guardian);
        //     _patientRepositoryMock.Setup(p => p.AddPatientAsync(It.IsAny<Patient>())).Returns(Task.FromResult(1));

        //     // Act
        //     var result = await _controller.AddPatient(patient);

        //     // Assert
        //     var createdResult = result.Result as CreatedAtActionResult;
        //     Assert.IsNotNull(createdResult);
        //     Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
        // }

        [TestMethod]
        public async Task DeletePatient_ReturnsNoContent_WhenPatientIsDeleted()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new Patient { ID = patientId, FirstName = "DefaultFirstName", LastName = "DefaultLastName", Avatar = "DefaultAvatar" };

            _patientRepositoryMock.Setup(p => p.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);
            _patientRepositoryMock.Setup(p => p.DeletePatientAsync(patientId)).ReturnsAsync(1);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeletePatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _patientRepositoryMock.Setup(p => p.GetPatientByIdAsync(patientId)).ReturnsAsync((Patient)null!);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }
    }
}