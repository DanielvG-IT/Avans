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
        public required Mock<ILogger<JournalController>> _mockLogger;
        public required Mock<IJournalRepository> _mockRepo;
        public required JournalController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IJournalRepository>();
            _mockLogger = new Mock<ILogger<JournalController>>();
            _controller = new JournalController(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsOkResult_WithListOfJournals()
        {
            // Arrange
            var journals = new List<JournalEntry> { new() { ID = Guid.NewGuid(), Content = "Test Content", Date = DateTime.UtcNow } };
            _mockRepo.Setup(repo => repo.GetAllJournalEntriesAsync()).ReturnsAsync(journals);

            // Act
            var result = await _controller.GetJournals();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<JournalEntry>));
            Assert.AreEqual(journals, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournal_ReturnsOkResult_WithJournal()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry { ID = journalId, Content = "Test Content", Date = DateTime.UtcNow };
            _mockRepo.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync(journal);

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(JournalEntry));
            Assert.AreEqual(journal, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournal_ReturnsNotFound_WhenJournalDoesNotExist()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.GetJournalEntryByIdAsync(journalId)).ReturnsAsync((JournalEntry?)null);

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
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
            Assert.IsInstanceOfType(createdAtActionResult.Value, typeof(JournalEntry));
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
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
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
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
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
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}