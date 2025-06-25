using Api.Controllers;
using Api.Interfaces;
using Api.Models;
using Api.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiTest
{
    [TestClass]
    public class LitterControllerTests
    {
        private Mock<ILitterRepository>? _mockRepo;
        private Mock<IFastApiPredictionService>? _mockPredictionService;
        private Mock<IHolidayApiService>? _mockHolidayService;
        private Mock<IWeatherService>? _mockWeatherService;
        private Mock<IDTOService>? _mockDtoService;
        private LitterController? _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ILitterRepository>();
            _mockPredictionService = new Mock<IFastApiPredictionService>();
            _mockHolidayService = new Mock<IHolidayApiService>();
            _mockWeatherService = new Mock<IWeatherService>();
            _mockDtoService = new Mock<IDTOService>();

            _controller = new LitterController(
                _mockRepo.Object,
                _mockPredictionService.Object,
                _mockHolidayService.Object,
                _mockWeatherService.Object,
                _mockDtoService.Object
            );
        }

        [TestMethod]
        public async Task Get_WhenNoLittersFound_ReturnsNotFound()
        {
            _mockRepo!.Setup(r => r.GetFilteredAsync(It.IsAny<LitterFilterDto>()))
                     .ReturnsAsync(new List<Litter>());

            var result = await _controller!.Get(new LitterFilterDto());

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Get_WhenLittersExist_ReturnsOk()
        {
            var litters = new List<Litter> { new Litter { Type = "plastic" } };
            _mockRepo!.Setup(r => r.GetFilteredAsync(It.IsAny<LitterFilterDto>()))
                     .ReturnsAsync(litters);

            var result = await _controller!.Get(new LitterFilterDto());

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(litters, okResult.Value);
        }

        [TestMethod]
        public async Task GetCameras_WhenNoCamerasFound_ReturnsNotFound()
        {
            _mockRepo!.Setup(r => r.GetCamerasAsync())
                     .ReturnsAsync(new List<Camera>());

            var result = await _controller!.GetCameras();
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetCameras_WhenCamerasExist_ReturnsOk()
        {
            var cameras = new List<Camera> { new Camera { Id = 1, Location = "TestLocation" } };
            _mockRepo!.Setup(r => r.GetCamerasAsync())
                     .ReturnsAsync(cameras);

            var result = await _controller!.GetCameras();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(cameras, ok.Value);
        }

        [TestMethod]
        public async Task Retrain_WhenServiceFails_ReturnsBadRequest()
        {
            // Arrange
            _mockPredictionService!.Setup(s => s.RetrainModelAsync(It.IsAny<int>()))
                                  .ReturnsAsync(false);

            // Act
            var result = await _controller!.Retrain(1);

            // Assert
            if (result is BadRequestObjectResult badRequest)
            {
                StringAssert.Contains(badRequest.Value?.ToString(), "Retrain failed");
            }
            else
            {
                Assert.Fail("Expected BadRequestObjectResult");
            }
        }

        [TestMethod]
        public async Task Retrain_WhenServiceSucceeds_ReturnsOk()
        {
            _mockPredictionService!.Setup(s => s.RetrainModelAsync(It.IsAny<int>()))
                                  .ReturnsAsync(true);

            var result = await _controller!.Retrain(1);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual("Model retrained successfully.", ok.Value);
        }

        [TestMethod]
        public async Task GetAmountPerCamera_WhenDataExists_ReturnsOk()
        {
            var data = new List<LitterAmountCamera> { new LitterAmountCamera { CameraId = 1 } };
            _mockRepo!.Setup(r => r.GetAmountPerCameraAsync())
                     .ReturnsAsync(data);

            var result = await _controller!.GetAmountPerCamera();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(data, ok.Value);
        }
    }
}