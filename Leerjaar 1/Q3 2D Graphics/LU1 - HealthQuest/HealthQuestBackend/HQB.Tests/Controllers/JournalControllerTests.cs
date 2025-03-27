using Moq;
using HQB.WebApi.Controllers;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace HQB.Tests.Controllers
{
    [TestClass]
    public class JournalControllerTests
    {
        public required Mock<IAuthenticationService> _mockAuthService;
        public required Mock<ILogger<JournalController>> _mockLogger;
        public required Mock<IGuardianRepository> _mockGuardianRepo;
        public required Mock<IPatientRepository> _mockPatientRepo;
        public required Mock<IJournalRepository> _mockRepo;
        public required JournalController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IJournalRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockGuardianRepo = new Mock<IGuardianRepository>();
            _mockLogger = new Mock<ILogger<JournalController>>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new JournalController(_mockLogger.Object, _mockRepo.Object, _mockAuthService.Object, _mockGuardianRepo.Object, _mockPatientRepo.Object);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsOkResult_WithJournalsForPatientId()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patientJournals = new List<JournalEntry> { new() { ID = Guid.NewGuid(), Content = "Patient Journal", Date = DateTime.UtcNow } };
            _mockRepo.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId)).ReturnsAsync(patientJournals);

            // Act
            var result = await _controller.GetJournals(null, patientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null. Ensure the controller returns OkObjectResult when journals are found.");
            Assert.IsInstanceOfType<IEnumerable<JournalEntry>>(okResult.Value, "Expected the result value to be of type IEnumerable<JournalEntry>.");
            Assert.AreEqual(patientJournals, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsNotFound_WhenNoJournalsForPatientId()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId)).ReturnsAsync([]);

            // Act
            var result = await _controller.GetJournals(null, patientId);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsOkResult_WithJournalsForGuardianId()
        {
            // Arrange
            var guardianId = Guid.NewGuid();
            var guardianJournals = new List<JournalEntry> { new() { ID = Guid.NewGuid(), Content = "Guardian Journal", Date = DateTime.UtcNow } };
            _mockRepo.Setup(repo => repo.GetJournalEntriesByGuardianIdAsync(guardianId)).ReturnsAsync(guardianJournals);

            // Act
            var result = await _controller.GetJournals(guardianId, null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType<IEnumerable<JournalEntry>>(okResult.Value);
            Assert.AreEqual(guardianJournals, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsNotFound_WhenNoJournalsForGuardianId()
        {
            // Arrange
            var guardianId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetJournalEntriesByGuardianIdAsync(guardianId)).ReturnsAsync([]);

            // Act
            var result = await _controller.GetJournals(guardianId, null);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsBadRequest_WhenLoggedInUserIdIsNull()
        {
            // Arrange
            _mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns((string?)null);

            // Act
            var result = await _controller.GetJournals(null, null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Unable to determine logged-in user ID", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostJournal_ReturnsCreatedAtActionResult_WithJournal()
        {
            // Arrange
            var journal = new JournalEntry { ID = Guid.NewGuid(), Content = "Test Content", Date = DateTime.UtcNow };
            _mockRepo.Setup(repo => repo.AddJournalEntryAsync(journal)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsInstanceOfType<JournalEntry>(createdAtActionResult.Value);
            Assert.AreEqual(journal, createdAtActionResult.Value);
        }

        [TestMethod]
        public async Task PutJournal_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry { ID = journalId, Date = DateTime.UtcNow, Content = "Test Content" };
            _mockRepo.Setup(repo => repo.UpdateJournalEntryAsync(journal)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutJournal(journalId, journal);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }

        [TestMethod]
        public async Task PutJournal_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry { ID = Guid.NewGuid(), Content = "Test Content", Date = DateTime.UtcNow };

            // Act
            var result = await _controller.PutJournal(journalId, journal);

            // Assert
            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task DeleteJournal_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.DeleteJournalEntryAsync(journalId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteJournal(journalId);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }
    }
}