using Api.Controllers;
using Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiTest
{
    [TestClass]
    public class TrashDTOControllerTests
    {
        private Mock<ITrashImportService>?_mockService;
        private TrashDTOController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<ITrashImportService>();
            _controller = new TrashDTOController(_mockService.Object);
        }

        [TestMethod] 
        public async Task StartImport_WhenImportSuccessful_ReturnsNoContent() //This verifies that when the import succeeds, the controller responds correctly with HTTP 204 No Content, meaning everything went fine but there is no body to return.


        {
            // Arrange
            _mockService!.Setup(s => s.ImportAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller!.StartImport();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task StartImport_WhenImportFails_ReturnsBadRequest() //checks that the controller correctly handles failures by returning HTTP 400 Bad Request with a clear message when the import service fails.
        {
            // Arrange
            _mockService!.Setup(s => s.ImportAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller!.StartImport();

            // Assert
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Importing Data failed!", badRequest.Value);
        }
    }
}