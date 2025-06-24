using System.Net;
using System.Net.Http.Json;
using Api.Interfaces;
using Api.Models;
using Api.Models.Data;
using Api.Models.Enums;
using Api.Services;
using Moq;
using Moq.Protected;

namespace ApiTest
{
    [TestClass]
    public class TestTrashImportService
    {
        private Mock<ILitterRepository> _mockRepo = null!;
        private Mock<IHolidayApiService> _mockHolidayApi = null!;
        private Mock<IDTOService> _mockDto = null!;
        private Mock<HttpMessageHandler> _mockHttpHandler = null!;
        private HttpClient _httpClient = null!;
        private TrashImportService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<ILitterRepository>();
            _mockHolidayApi = new Mock<IHolidayApiService>();
            _mockDto = new Mock<IDTOService>();
            _mockHttpHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("https://example.com")
            };

            _service = new TrashImportService(
                _mockRepo.Object,
                _mockHolidayApi.Object,
                _mockDto.Object,
                _httpClient
            );
        }

        [TestMethod]
        public async Task GetStatusAsync_ReturnsTrue_WhenHttpClientReturnsSuccess()
        {
            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var result = await _service.GetStatusAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetStatusAsync_ReturnsFalse_OnHttpException()
        {
            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException());

            var result = await _service.GetStatusAsync();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ImportAsync_ReturnsFalse_WhenLitterRepositoryThrows()
        {
            _mockRepo.Setup(r => r.GetLatestLitterTimeAsync(2))
                     .ThrowsAsync(new Exception("DB error"));

            var result = await _service.ImportAsync(CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ImportAsync_ReturnsFalse_WhenHttpRequestFails()
        {
            _mockRepo.Setup(r => r.GetLatestLitterTimeAsync(2))
                     .ReturnsAsync(DateTime.Now);

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("HTTP failed"));

            var result = await _service.ImportAsync(CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ImportAsync_ReturnsFalse_WhenNoResults()
        {
            _mockRepo.Setup(r => r.GetLatestLitterTimeAsync(2))
                     .ReturnsAsync(DateTime.Now);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AggregatedTrashDto { Litters = [] })
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var result = await _service.ImportAsync(CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ImportAsync_ReturnsTrue_WhenValidData()
        {
            var trash = new TrashDTO
            {
                Id = "1",
                Date = DateTime.Now,
                Confidence = 0.9,
                Temperature = 21.2f,
                Weather = "cloudy",
                Type = "plastic",
                Latitude = 0f,
                Longitude = 0f
            };

            _mockRepo.Setup(r => r.GetLatestLitterTimeAsync(2))
                     .ReturnsAsync(DateTime.Now.AddHours(-1));

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new AggregatedTrashDto { Litters = [trash] })
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            _mockHolidayApi
                .Setup(x => x.IsHolidayAsync(It.IsAny<DateTime>(), "NL", It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockDto.Setup(x => x.GetCategory("plastic")).Returns(LitterCategory.Plastic);
            _mockDto.Setup(x => x.GetWeatherCategory("cloudy")).Returns(WeatherCategory.Cloudy);

            _mockRepo.Setup(x => x.AddAsync(It.IsAny<List<Litter>>())).ReturnsAsync(true);

            var result = await _service.ImportAsync(CancellationToken.None);

            Assert.IsTrue(result);
        }
    }
}