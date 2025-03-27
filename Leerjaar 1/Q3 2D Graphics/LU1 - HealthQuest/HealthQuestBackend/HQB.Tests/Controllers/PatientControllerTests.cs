using Moq;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class PatientControllerTest
    {
        public required PatientController _controller;
        public required Mock<IPatientRepository> _mockRepo;
        public required Mock<IDoctorRepository> _mockDoctorRepo;
        public required Mock<IJournalRepository> _mockJournalRepo;
        public required Mock<IGuardianRepository> _mockGuardianRepo;
        public required Mock<IStickersRepository> _mockStickersRepo;
        public required Mock<ILogger<PatientController>> _mockLogger;
        public required Mock<ITreatmentRepository> _mockTreatmentRepo;
        public required Mock<IAuthenticationService> _mockAuthService;
        public required Mock<ICompletedAppointmentsRepository> _mockCompletedAppointmentsRepo;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IPatientRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockJournalRepo = new Mock<IJournalRepository>();
            _mockGuardianRepo = new Mock<IGuardianRepository>();
            _mockStickersRepo = new Mock<IStickersRepository>();
            _mockLogger = new Mock<ILogger<PatientController>>();
            _mockTreatmentRepo = new Mock<ITreatmentRepository>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockCompletedAppointmentsRepo = new Mock<ICompletedAppointmentsRepository>();

            _controller = new PatientController(
                _mockLogger.Object,
                _mockDoctorRepo.Object,
                _mockRepo.Object,
                _mockJournalRepo.Object,
                _mockStickersRepo.Object,
                _mockGuardianRepo.Object,
                _mockTreatmentRepo.Object,
                _mockAuthService.Object,
                _mockCompletedAppointmentsRepo.Object
            );
        }

        [TestMethod]
        public async Task GetAllPatients_ReturnsOkResult_WithListOfPatients()
        {
            // Arrange
            var patients = new List<Patient> { new() { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" } };
            _mockRepo.Setup(repo => repo.GetPatientsByGuardianId(It.IsAny<Guid>())).ReturnsAsync(patients);
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" };
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(Guid.NewGuid().ToString());
            _mockGuardianRepo.Setup(repo => repo.GetGuardianByUserIdAsync(It.IsAny<string>())).ReturnsAsync(guardian);

            // Act
            var result = await _controller.GetPatientsForCurrentUser();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(patients, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatientById_ReturnsOkResult_WithPatient()
        {
            // Arrange
            var patient = new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" };
            _mockRepo.Setup(repo => repo.GetPatientByIdAsync(It.IsAny<Guid>())).ReturnsAsync(patient);

            // Act
            var result = await _controller.GetPatientById(Guid.NewGuid());

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(patient, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatientById_ReturnsNotFoundResult_WhenPatientNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetPatientByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Patient?)null);

            // Act
            var result = await _controller.GetPatientById(Guid.NewGuid());

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task AddPatient_ReturnsCreatedAtActionResult_WithPatient()
        {
            // Arrange
            var patient = new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" };
            var guardian = new Guardian { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" };
            _mockRepo.Setup(repo => repo.AddPatientAsync(It.IsAny<Patient>())).ReturnsAsync(1);
            _mockGuardianRepo.Setup(repo => repo.GetGuardianByIdAsync(It.IsAny<Guid>())).ReturnsAsync(guardian);
            _mockGuardianRepo.Setup(repo => repo.GetGuardianByUserIdAsync(It.IsAny<string>())).ReturnsAsync(guardian); // <-- Add this line
            _mockAuthService.Setup(service => service.GetCurrentAuthenticatedUserId()).Returns(Guid.NewGuid().ToString());
            _mockRepo.Setup(repo => repo.GetPatientByIdAsync(It.IsAny<Guid>())).ReturnsAsync(patient);

            // Act
            var result = await _controller.AddPatient(patient);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(patient, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task UpdatePatient_ReturnsNoContentResult()
        {
            // Arrange
            var patient = new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" };
            _mockRepo.Setup(repo => repo.GetPatientByIdAsync(It.IsAny<Guid>())).ReturnsAsync(patient);
            _mockRepo.Setup(repo => repo.UpdatePatientAsync(It.IsAny<Patient>())).ReturnsAsync(1);

            // Act
            var result = await _controller.UpdatePatient(Guid.NewGuid(), patient);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeletePatient_ReturnsNoContentResult()
        {
            // Arrange
            var patient = new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" };
            _mockRepo.Setup(repo => repo.GetPatientByIdAsync(It.IsAny<Guid>())).ReturnsAsync(patient);
            _mockRepo.Setup(repo => repo.DeletePatientAsync(It.IsAny<Guid>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeletePatient(Guid.NewGuid());

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}