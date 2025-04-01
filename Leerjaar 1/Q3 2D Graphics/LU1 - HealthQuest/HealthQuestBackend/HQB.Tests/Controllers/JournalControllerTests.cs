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
        public required Mock<IAuthenticationService> _mockAuthenticationService;
        public required Mock<IGuardianRepository> _mockGuardianRepository;
        public required Mock<IJournalRepository> _mockJournalRepository;
        public required Mock<ILogger<JournalController>> _mockLogger;
        public required Mock<IPatientRepository> _mockPatientRepo;
        public required JournalController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockLogger = new Mock<ILogger<JournalController>>();
            _mockJournalRepository = new Mock<IJournalRepository>();
            _mockGuardianRepository = new Mock<IGuardianRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _controller = new JournalController(_mockLogger.Object, _mockJournalRepository.Object, _mockAuthenticationService.Object, _mockGuardianRepository.Object, _mockPatientRepo.Object);
        }

        [TestMethod]
        public async Task GetJournals_WithValidPatientId_ReturnsOkResult()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var journalEntries = new List<JournalEntry>
            {
                new() { ID = Guid.NewGuid(), PatientID = patientId, Date = DateTime.UtcNow, Content = "Sample content", Title = "Sample Title", Rating = 5 },
                new() { ID = Guid.NewGuid(), PatientID = patientId, Date = DateTime.UtcNow.AddDays(1), Content = "Another content", Title = "Another Title", Rating = 4 }
            };

            _mockJournalRepository.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId)).ReturnsAsync(journalEntries);

            // Act
            var result = await _controller.GetJournals(null, patientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(journalEntries, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournals_WithInvalidPatientId_ReturnsNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockJournalRepository.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId)).ReturnsAsync((IEnumerable<JournalEntry>)null!);

            // Act
            var result = await _controller.GetJournals(null, patientId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetJournal_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var guardianId = Guid.NewGuid();
            var loggedInUserId = "user123";
            var journalEntry = new JournalEntry
            {
                ID = journalId,
                GuardianID = guardianId,
                Date = DateTime.UtcNow,
                Rating = 5,
                Content = "Sample content",
                Title = "Sample Title"
            };

            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(loggedInUserId);
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync(loggedInUserId)).ReturnsAsync(new Guardian { ID = guardianId, FirstName = "John", LastName = "Doe" });
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync(journalEntry);

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(journalEntry, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournal_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var loggedInUserId = "user123";

            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(loggedInUserId);
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync(loggedInUserId)).ReturnsAsync(new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" });
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync((JournalEntry)null!);

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task PostJournal_WithNullJournal_ReturnsBadRequest()
        {
            // Arrange
            JournalEntry journal = null!;

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Journal entry cannot be null", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostJournal_WithMissingPatientID_ReturnsBadRequest()
        {
            // Arrange
            var journal = new JournalEntry
            {
                GuardianID = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Title = "Sample Title",
                Rating = 5,
                Content = "Sample content"
            };

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("PatientID is required", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostJournal_WithMissingGuardianID_ReturnsBadRequest()
        {
            // Arrange
            var journal = new JournalEntry
            {
                PatientID = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Title = "Sample Title",
                Rating = 5,
                Content = "Sample content"
            };

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("GuardianID is required", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostJournal_WithMissingTitle_ReturnsBadRequest()
        {
            // Arrange
            var journal = new JournalEntry
            {
                PatientID = Guid.NewGuid(),
                GuardianID = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Title = null!, // Missing title
                Rating = 5,
                Content = "Sample content"
            };

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Title is required", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PostJournal_WithInvalidRating_ReturnsBadRequest()
        {
            // Arrange
            var journal = new JournalEntry
            {
                PatientID = Guid.NewGuid(),
                GuardianID = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Title = "Sample Title",
                Content = "Sample content",
                Rating = 11 // Invalid rating
            };

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Rating must be between 1 and 10", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PutJournal_WithValidId_ReturnsOk()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var guardianId = Guid.NewGuid();
            var loggedInUserId = "user123";
            var existingJournal = new JournalEntry
            {
                ID = journalId,
                GuardianID = guardianId,
                Date = DateTime.UtcNow,
                Title = "Original Title",
                Content = "Original content",
                Rating = 10,
            };

            var updatedJournal = new JournalEntry
            {
                ID = journalId,
                GuardianID = guardianId,
                Date = DateTime.UtcNow,
                Rating = 5,
                Content = "Updated content",
                Title = "Updated Title"
            };

            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(loggedInUserId);
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync(loggedInUserId)).ReturnsAsync(new Guardian { ID = guardianId, FirstName = "DefaultFirstName", LastName = "DefaultLastName" });
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync(existingJournal);
            _mockJournalRepository.Setup(repo => repo.UpdateJournalEntryAsync(updatedJournal)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutJournal(journalId, updatedJournal);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(updatedJournal, okResult.Value);
        }

        // ...existing code...

        [TestMethod]
        public async Task PutJournal_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var updatedJournal = new JournalEntry
            {
                ID = Guid.NewGuid(), // Different from journalId
                GuardianID = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Rating = 5,
                Title = "Mismatched ID Title",
                Content = "Mismatched ID content"
            };

            // Act
            var result = await _controller.PutJournal(journalId, updatedJournal);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteJournal_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var guardianId = Guid.NewGuid();
            var loggedInUserId = "user123";
            var journalEntry = new JournalEntry
            {
                ID = journalId,
                GuardianID = guardianId,
                Date = DateTime.UtcNow,
                Rating = 5,
                Title = "Sample Title",
                Content = "Sample content"
            };

            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(loggedInUserId);
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync(loggedInUserId)).ReturnsAsync(new Guardian { ID = guardianId, FirstName = "DefaultFirstName", LastName = "DefaultLastName" });
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync(journalEntry);
            _mockJournalRepository.Setup(repo => repo.DeleteJournalEntryAsync(journalId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteJournal(journalId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }
    }
}