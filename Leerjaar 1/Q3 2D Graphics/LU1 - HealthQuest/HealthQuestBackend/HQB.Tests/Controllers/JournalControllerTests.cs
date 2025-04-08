using HQB.WebApi.Controllers;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Models;
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
    public class JournalControllerTests
    {
        public required Mock<IAuthenticationService> _mockAuthenticationService;
        public required Mock<IGuardianRepository> _mockGuardianRepository;
        public required Mock<IJournalRepository> _mockJournalRepository;
        public required Mock<IPatientRepository> _mockPatientRepository;
        public required Mock<ILogger<JournalController>> _mockLogger;
        public required JournalController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockJournalRepository = new Mock<IJournalRepository>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockGuardianRepository = new Mock<IGuardianRepository>();
            _mockPatientRepository = new Mock<IPatientRepository>();
            _mockLogger = new Mock<ILogger<JournalController>>();

            _controller = new JournalController(
                _mockLogger.Object,
                _mockJournalRepository.Object,
                _mockAuthenticationService.Object,
                _mockGuardianRepository.Object,
                _mockPatientRepository.Object
            );
        }

        [TestMethod]
        public async Task GetJournals_ReturnsOk_WhenJournalsExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var journals = new List<JournalEntry>
            {
                new() { ID = Guid.NewGuid(), PatientID = patientId, Title = "Test", Content = "Content", Rating = 5, Date = DateTime.UtcNow.ToString("yyyy-MM-dd") }
            };
            _mockJournalRepository.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId))
                .ReturnsAsync(journals);

            // Act
            var result = await _controller.GetJournals(patientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(journals, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournals_ReturnsNotFound_WhenNoJournalsExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            _mockJournalRepository.Setup(repo => repo.GetJournalEntriesByPatientIdAsync(patientId))
                .ReturnsAsync([]);

            // Act
            var result = await _controller.GetJournals(patientId);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task GetJournal_ReturnsOk_WhenJournalExists()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry { ID = journalId, Title = "Test", Content = "Content", Rating = 5, Date = DateTime.UtcNow.ToString("yyyy-MM-dd") };
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId))
                .ReturnsAsync(journal);
            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                .Returns("user123");
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync("user123"))
                .ReturnsAsync(new Guardian { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" });

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(journal, okResult.Value);
        }

        [TestMethod]
        public async Task GetJournal_ReturnsNotFound_WhenJournalDoesNotExist()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId))
                .ReturnsAsync((JournalEntry)null!);
            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                .Returns("user123");
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync("user123"))
                .ReturnsAsync(new Guardian { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" });

            // Act
            var result = await _controller.GetJournal(journalId);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(result.Result);
        }

        [TestMethod]
        public async Task PostJournal_ReturnsCreated_WhenJournalIsValid()
        {
            // Arrange
            var journal = new JournalEntry
            {
                PatientID = Guid.NewGuid(),
                Title = "Test",
                Content = "Content",
                Rating = 5,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            _mockJournalRepository.Setup(repo => repo.AddJournalEntryAsync(It.IsAny<JournalEntry>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostJournal(journal);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(journal, createdResult.Value);
        }

        [TestMethod]
        public async Task PutJournal_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry
            {
                ID = journalId,
                PatientID = Guid.NewGuid(),
                Title = "Updated Title",
                Content = "Updated Content",
                Rating = 8,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId))
                .ReturnsAsync(journal);
            _mockJournalRepository.Setup(repo => repo.UpdateJournalEntryAsync(journal))
                .Returns(Task.CompletedTask);
            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                .Returns("user123");
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync("user123"))
                .ReturnsAsync(new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" });

            // Act
            var result = await _controller.PutJournal(journalId, journal);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(journal, okResult.Value);
        }

        [TestMethod]
        public async Task DeleteJournal_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var journalId = Guid.NewGuid();
            var journal = new JournalEntry
            {
                ID = journalId,
                Title = "Default Title",
                Content = "Default Content",
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            _mockJournalRepository.Setup(repo => repo.GetJournalEntryByIdAsync(journalId))
                .ReturnsAsync(journal);
            _mockJournalRepository.Setup(repo => repo.DeleteJournalEntryAsync(journalId))
                .Returns(Task.CompletedTask);
            _mockAuthenticationService.Setup(auth => auth.GetCurrentAuthenticatedUserId())
                .Returns("user123");
            _mockGuardianRepository.Setup(repo => repo.GetGuardianByUserIdAsync("user123"))
                .ReturnsAsync(new Guardian { ID = Guid.NewGuid(), FirstName = "DefaultFirstName", LastName = "DefaultLastName" });

            // Act
            var result = await _controller.DeleteJournal(journalId);

            // Assert
            Assert.IsInstanceOfType<NoContentResult>(result);
        }
    }
}