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
    public class PatientControllerTests
    {
        public required PatientController _controller;
        public required Mock<ILogger<PatientController>> _mockLogger;
        public required Mock<IDoctorRepository> _mockDoctorRepository;
        public required Mock<IPatientRepository> _mockPatientRepository;
        public required Mock<IJournalRepository> _mockJournalRepository;
        public required Mock<IStickersRepository> _mockStickersRepository;
        public required Mock<IGuardianRepository> _mockGuardianRepository;
        public required Mock<ITreatmentRepository> _mockTreatmentRepository;
        public required Mock<IAuthenticationService> _mockAuthenticationService;
        public required Mock<ICompletedAppointmentsRepository> _mockCompletedAppointmentsRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<PatientController>>();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockJournalRepository = new Mock<IJournalRepository>();
            _mockStickersRepository = new Mock<IStickersRepository>();
            _mockGuardianRepository = new Mock<IGuardianRepository>();
            _mockTreatmentRepository = new Mock<ITreatmentRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockCompletedAppointmentsRepository = new Mock<ICompletedAppointmentsRepository>();

            _controller = new PatientController(
                _mockLogger.Object,
                _mockDoctorRepository.Object,
                _mockPatientRepository.Object,
                _mockJournalRepository.Object,
                _mockStickersRepository.Object,
                _mockGuardianRepository.Object,
                _mockTreatmentRepository.Object,
                _mockAuthenticationService.Object,
                _mockCompletedAppointmentsRepository.Object
            );
        }

        [TestMethod]
        public async Task GetPatientsForCurrentUser_ReturnsOk_WhenPatientsExist()
        {
            // Arrange
            var userId = "test-user-id";
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" };
            var patients = new List<Patient> { new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "default-avatar.png" } };

            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns(userId);
            _mockGuardianRepository.Setup(r => r.GetGuardianByUserIdAsync(userId)).ReturnsAsync(guardian);
            _mockPatientRepository.Setup(r => r.GetPatientsByGuardianId(guardian.ID)).ReturnsAsync(patients);

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
            _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns((string)null!);

            // Act
            var result = await _controller.GetPatientsForCurrentUser();

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task GetPatientById_ReturnsOk_WhenPatientExists()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new Patient { ID = patientId, FirstName = "John", LastName = "Doe", Avatar = "default-avatar.png" };

            _mockPatientRepository.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);

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

            _mockPatientRepository.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync((Patient)null!);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // [TestMethod]
        // public async Task AddPatient_ReturnsCreated_WhenPatientIsAdded()
        // {
        //     // Arrange
        //     var patient = new Patient { FirstName = "John", LastName = "Doe", Avatar = "default-avatar.png", DoctorID = Guid.NewGuid(), TreatmentID = Guid.NewGuid() };
        //     var userId = "test-user-id";
        //     var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" };

        //     _mockAuthenticationService.Setup(s => s.GetCurrentAuthenticatedUserId()).Returns(userId);
        //     _mockGuardianRepository.Setup(r => r.GetGuardianByUserIdAsync(userId)).ReturnsAsync(guardian);
        //     if (guardian == null)
        //     {
        //         _mockPatientRepository.Setup(r => r.AddPatientAsync(It.IsAny<Patient>())).Throws(new Exception("Guardian with ID not found"));
        //     }
        //     else
        //     {
        //         _mockPatientRepository.Setup(r => r.AddPatientAsync(It.Is<Patient>(p => p.GuardianID == guardian.ID))).Returns(Task.FromResult(1));
        //     }

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
            var patient = new Patient { ID = patientId, FirstName = "DefaultFirstName", LastName = "DefaultLastName", Avatar = "default-avatar.png" };

            _mockPatientRepository.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);
            _mockPatientRepository.Setup(r => r.DeletePatientAsync(patientId)).ReturnsAsync(1);

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

            _mockPatientRepository.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync((Patient)null!);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }
    }
}