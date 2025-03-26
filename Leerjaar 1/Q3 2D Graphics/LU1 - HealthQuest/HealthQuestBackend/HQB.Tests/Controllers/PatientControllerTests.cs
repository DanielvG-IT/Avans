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
        public required Mock<IPatientRepository> _mockRepo;
        public required PatientController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IPatientRepository>();
            var mockLogger = new Mock<ILogger<PatientController>>();
            _controller = new PatientController(_mockRepo.Object, mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllPatients_ReturnsOkResult_WithListOfPatients()
        {
            // Arrange
            var patients = new List<Patient> { new() { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" } };
            _mockRepo.Setup(repo => repo.GetAllPatientsAsync()).ReturnsAsync(patients);

            // Act
            var result = await _controller.GetAllPatients();

            // Assert
            var okResult = result as OkObjectResult;
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
            var okResult = result as OkObjectResult;
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
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task AddPatient_ReturnsCreatedAtActionResult_WithPatient()
        {
            // Arrange
            var patient = new Patient { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Avatar = "defaultAvatar.png" };
            _mockRepo.Setup(repo => repo.AddPatientAsync(It.IsAny<Patient>())).ReturnsAsync(1);

            // Act
            var result = await _controller.AddPatient(patient);

            // Assert
            var createdAtActionResult = result as CreatedAtActionResult;
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