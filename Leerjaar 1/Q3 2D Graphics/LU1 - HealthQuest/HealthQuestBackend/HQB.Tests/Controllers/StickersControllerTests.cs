using Moq;
using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HQB.Tests.Controllers
{
  [TestClass]
  public class StickersControllerTests
  {
    public required StickersController _controller;
    public required Mock<IStickersRepository> _mockRepository;
    public required Mock<ILogger<StickersController>> _mockLogger;

    [TestInitialize]
    public void Setup()
    {
      _mockRepository = new Mock<IStickersRepository>();
      _mockLogger = new Mock<ILogger<StickersController>>();
      _controller = new StickersController(_mockRepository.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetAllStickers_ReturnsOkResult_WithListOfStickers()
    {
      // Arrange
      var stickers = new List<Sticker> { new() { Id = Guid.NewGuid(), Name = "Sticker1" } };
      _mockRepository.Setup(repo => repo.GetAllStickersAsync()).ReturnsAsync(stickers);

      // Act
      var result = await _controller.GetAllStickers();

      // Assert
      var okResult = result.Result as OkObjectResult;
      Assert.IsNotNull(okResult);
      Assert.AreEqual(200, okResult.StatusCode);
      Assert.AreEqual(stickers, okResult.Value);
    }

    [TestMethod]
    public async Task GetAllStickers_ReturnsNotFound_WhenNoStickersExist()
    {
      // Arrange
      _mockRepository.Setup(repo => repo.GetAllStickersAsync()).ReturnsAsync((IEnumerable<Sticker>)null!);

      // Act
      var result = await _controller.GetAllStickers();

      // Assert
      var notFoundResult = result.Result as NotFoundObjectResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public async Task AddSticker_ReturnsCreatedAtAction_WhenStickerIsValid()
    {
      // Arrange
      var sticker = new Sticker { Name = "New Sticker" };
      _mockRepository.Setup(repo => repo.AddStickerAsync(It.IsAny<Sticker>())).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.AddSticker(sticker);

      // Assert
      var createdResult = result.Result as CreatedAtActionResult;
      Assert.IsNotNull(createdResult);
      Assert.AreEqual(201, createdResult.StatusCode);
      Assert.AreEqual(sticker, createdResult.Value);
    }

    [TestMethod]
    public async Task AddSticker_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
      // Arrange
      _controller.ModelState.AddModelError("Name", "Required");

      // Act
      var result = await _controller.AddSticker(new Sticker { Name = "Test Sticker" });

      // Assert
      var badRequestResult = result.Result as BadRequestObjectResult;
      Assert.IsNotNull(badRequestResult);
      Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task GetStickerById_ReturnsOkResult_WhenStickerExists()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      var sticker = new Sticker { Id = stickerId, Name = "Sticker1" };
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync(sticker);

      // Act
      var result = await _controller.GetStickerById(stickerId);

      // Assert
      var okResult = result.Result as OkObjectResult;
      Assert.IsNotNull(okResult);
      Assert.AreEqual(200, okResult.StatusCode);
      Assert.AreEqual(sticker, okResult.Value);
    }

    [TestMethod]
    public async Task GetStickerById_ReturnsNotFound_WhenStickerDoesNotExist()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync((Sticker)null!);

      // Act
      var result = await _controller.GetStickerById(stickerId);

      // Assert
      var notFoundResult = result.Result as NotFoundResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateSticker_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      var sticker = new Sticker { Id = stickerId, Name = "Updated Sticker" };
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync(sticker);
      _mockRepository.Setup(repo => repo.UpdateStickerAsync(sticker)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.UpdateSticker(stickerId, sticker);

      // Assert
      var noContentResult = result as NoContentResult;
      Assert.IsNotNull(noContentResult);
      Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task UpdateSticker_ReturnsNotFound_WhenStickerDoesNotExist()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      var sticker = new Sticker { Id = stickerId, Name = "Updated Sticker" };
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync((Sticker)null!);

      // Act
      var result = await _controller.UpdateSticker(stickerId, sticker);

      // Assert
      var notFoundResult = result as NotFoundResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSticker_ReturnsNoContent_WhenDeleteIsSuccessful()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      var sticker = new Sticker { Id = stickerId, Name = "Sticker1" };
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync(sticker);
      _mockRepository.Setup(repo => repo.DeleteStickerAsync(stickerId)).Returns(Task.CompletedTask);

      // Act
      var result = await _controller.DeleteSticker(stickerId);

      // Assert
      var noContentResult = result as NoContentResult;
      Assert.IsNotNull(noContentResult);
      Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSticker_ReturnsNotFound_WhenStickerDoesNotExist()
    {
      // Arrange
      var stickerId = Guid.NewGuid();
      _mockRepository.Setup(repo => repo.GetStickerByIdAsync(stickerId)).ReturnsAsync((Sticker)null!);

      // Act
      var result = await _controller.DeleteSticker(stickerId);

      // Assert
      var notFoundResult = result as NotFoundResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public async Task SearchStickersByName_ReturnsOkResult_WhenStickerIsFound()
    {
      // Arrange
      var stickerName = "Sticker1";
      var stickers = new List<Sticker> { new() { Id = Guid.NewGuid(), Name = stickerName } };
      _mockRepository.Setup(repo => repo.GetAllStickersAsync()).ReturnsAsync(stickers);

      // Act
      var result = await _controller.SearchStickersByName(stickerName);

      // Assert
      var okResult = result.Result as OkObjectResult;
      Assert.IsNotNull(okResult);
      Assert.AreEqual(200, okResult.StatusCode);
      Assert.AreEqual(stickers.First(), okResult.Value);
    }

    [TestMethod]
    public async Task SearchStickersByName_ReturnsNotFound_WhenStickerIsNotFound()
    {
      // Arrange
      var stickerName = "NonExistentSticker";
      _mockRepository.Setup(repo => repo.GetAllStickersAsync()).ReturnsAsync([]);

      // Act
      var result = await _controller.SearchStickersByName(stickerName);

      // Assert
      var notFoundResult = result.Result as NotFoundObjectResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
    }
  }
}