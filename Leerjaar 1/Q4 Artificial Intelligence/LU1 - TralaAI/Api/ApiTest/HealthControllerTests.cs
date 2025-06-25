using Api.Controllers;
using Api.Interfaces;
using Api.Models;
using Api.Models.Health;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiTest
{
    [TestClass]
    public class HealthControllerTests
    {
        private Mock<IFastApiPredictionService> _mockFastApi = null!;
        private Mock<IHolidayApiService> _mockHolidayApi = null!;
        private Mock<IWeatherService> _mockWeatherApi = null!;
        private Mock<ITrashImportService> _mockTrashService = null!;
        private Health _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockFastApi = new Mock<IFastApiPredictionService>();
            _mockHolidayApi = new Mock<IHolidayApiService>();
            _mockWeatherApi = new Mock<IWeatherService>();
            _mockTrashService = new Mock<ITrashImportService>();

            _controller = new Health(
                _mockFastApi.Object,
                _mockHolidayApi.Object,
                _mockWeatherApi.Object,
                _mockTrashService.Object
            );
        }

        [TestMethod]
        public void GetStatus_ReturnsHealthy()
        {
            var result = _controller.GetStatus();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var status = ok!.Value as HealthStatus;
            Assert.AreEqual("Healthy", status!.Status);
        }

        [TestMethod]
        public async Task GetFastApiStatus_Healthy_ReturnsOk()
        {
            _mockFastApi.Setup(s => s.GetStatusAsync()).ReturnsAsync(true);

            var result = await _controller.GetFastApiStatus();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var status = ok!.Value as HealthStatus;
            Assert.AreEqual("Fast API is healthy", status!.Status);
        }

        [TestMethod]
        public async Task GetFastApiStatus_Unhealthy_Returns503()
        {
            _mockFastApi.Setup(s => s.GetStatusAsync()).ReturnsAsync(false);

            var result = await _controller.GetFastApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(503, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetFastApiStatus_Throws_Returns500()
        {
            _mockFastApi.Setup(s => s.GetStatusAsync()).ThrowsAsync(new Exception("boom"));

            var result = await _controller.GetFastApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(500, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetFastApiModelStatus_ModelFound_ReturnsOk()
        {
            var modelStatus = new ModelStatusResponse
            {
                Status = "model ok",
            };

            _mockFastApi.Setup(s => s.GetModelStatus(1))
                        .ReturnsAsync(modelStatus);

            var result = await _controller.GetFastApiStatus(1);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(modelStatus, ok!.Value);
        }

        [TestMethod]
        public async Task GetFastApiModelStatus_ModelNull_Returns503()
        {
            _mockFastApi.Setup(s => s.GetModelStatus(1))
                        .ReturnsAsync((ModelStatusResponse?)null);

            var result = await _controller.GetFastApiStatus(1);

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(503, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetFastApiModelStatus_Throws_Returns500()
        {
            _mockFastApi.Setup(s => s.GetModelStatus(1)).ThrowsAsync(new Exception("kapot"));

            var result = await _controller.GetFastApiStatus(1);

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(500, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetHolidayApiStatus_Valid_ReturnsOk()
        {
            _mockHolidayApi.Setup(s => s.IsHolidayAsync(It.IsAny<DateTime>(), "NL", It.IsAny<string>()))
                           .ReturnsAsync(true);

            var result = await _controller.GetHolidayApiStatus();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var status = ok!.Value as HealthStatus;
            Assert.AreEqual("Holiday API is healthy", status!.Status);
        }

        [TestMethod]
        public async Task GetHolidayApiStatus_Null_Returns503()
        {
            _mockHolidayApi.Setup(s => s.IsHolidayAsync(It.IsAny<DateTime>(), "NL", It.IsAny<string>()))
                           .ReturnsAsync((bool?)null);

            var result = await _controller.GetHolidayApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(503, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetHolidayApiStatus_Throws_Returns500()
        {
            _mockHolidayApi.Setup(s => s.IsHolidayAsync(It.IsAny<DateTime>(), "NL", It.IsAny<string>()))
                           .ThrowsAsync(new Exception("fout"));

            var result = await _controller.GetHolidayApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(500, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetWeatherApiStatus_Healthy_ReturnsOk()
        {
            _mockWeatherApi.Setup(s => s.GetStatusAsync()).ReturnsAsync(true);

            var result = await _controller.GetWeatherApiStatus();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var status = ok!.Value as HealthStatus;
            Assert.AreEqual("Weather API is healthy", status!.Status);
        }

        [TestMethod]
        public async Task GetWeatherApiStatus_Unhealthy_Returns503()
        {
            _mockWeatherApi.Setup(s => s.GetStatusAsync()).ReturnsAsync(false);

            var result = await _controller.GetWeatherApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(503, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetWeatherApiStatus_Throws_Returns500()
        {
            _mockWeatherApi.Setup(s => s.GetStatusAsync()).ThrowsAsync(new Exception("kapot"));

            var result = await _controller.GetWeatherApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(500, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetSensoringApiStatus_Healthy_ReturnsOk()
        {
            _mockTrashService.Setup(s => s.GetStatusAsync()).ReturnsAsync(true);

            var result = await _controller.GetSensoringApiStatus();

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);

            var status = ok!.Value as HealthStatus;
            Assert.AreEqual("Sensoring API is healthy", status!.Status);
        }

        [TestMethod]
        public async Task GetSensoringApiStatus_Unhealthy_Returns503()
        {
            _mockTrashService.Setup(s => s.GetStatusAsync()).ReturnsAsync(false);

            var result = await _controller.GetSensoringApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(503, res!.StatusCode);
        }

        [TestMethod]
        public async Task GetSensoringApiStatus_Throws_Returns500()
        {
            _mockTrashService.Setup(s => s.GetStatusAsync()).ThrowsAsync(new Exception("stuk"));

            var result = await _controller.GetSensoringApiStatus();

            var res = result as ObjectResult;
            Assert.IsNotNull(res);
            Assert.AreEqual(500, res!.StatusCode);
        }
    }
}