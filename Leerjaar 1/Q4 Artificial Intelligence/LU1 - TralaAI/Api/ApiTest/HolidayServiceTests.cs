using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Net;
using Api.Services;
using Api.Models;
using Moq.Protected;
using Moq;

namespace ApiTest
{
  [TestClass]
  public class HolidayApiServiceTests
  {
    private static Mock<IMemoryCache> SetupMemoryCache(object key, object value)
    {
      var mockCache = new Mock<IMemoryCache>();

      mockCache.Setup(m => m.TryGetValue(key, out It.Ref<object>.IsAny!))
               .Callback((object k, out object v) =>
               {
                 v = value ?? (object)new List<HolidayApiResponse>();
               })
               .Returns((object k, out object v) =>
               {
                 v = value ?? (object)new List<HolidayApiResponse>();
                 return key.Equals(k);
               });

      mockCache.Setup(m => m.CreateEntry(It.IsAny<object>()))
               .Returns(Mock.Of<ICacheEntry>());

      return mockCache;
    }

    private delegate void TryGetValueCallback(object key, out object value);

    private static HttpClient SetupHttpClient(HttpStatusCode statusCode, string content)
    {
      var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

      handler.Protected()
          .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(new HttpResponseMessage
          {
            StatusCode = statusCode,
            Content = new StringContent(content)
          });

      return new HttpClient(handler.Object)
      {
        BaseAddress = new Uri("http://localhost")
      };
    }

    [TestMethod]
    public async Task IsHolidayAsync_ReturnsTrue_WhenDateIsHoliday()
    {
      // Arrange
      var year = "2024";
      var countryCode = "US";
      var date = new DateTime(2024, 7, 4);
      var holidays = new List<HolidayApiResponse>
            {
                new() { Date = "2024-07-04", CountryCode = "US" }
            };
      var yearCacheKey = $"holidays:{countryCode}:{year}";
      var cache = SetupMemoryCache(yearCacheKey, holidays);
      var httpClient = SetupHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(holidays));
      var service = new HolidayApiService(httpClient, cache.Object);

      // Act
      var result = await service.IsHolidayAsync(date, countryCode, year);

      // Assert
      Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task IsHolidayAsync_ReturnsFalse_WhenDateIsNotHoliday()
    {
      // Arrange
      var year = "2024";
      var countryCode = "US";
      var date = new DateTime(2024, 7, 5);
      var holidays = new List<HolidayApiResponse>
            {
                new() { Date = "2024-07-04", CountryCode = "US" }
            };
      var yearCacheKey = $"holidays:{countryCode}:{year}";
      var cache = SetupMemoryCache(yearCacheKey, holidays);
      var httpClient = SetupHttpClient(HttpStatusCode.OK, JsonSerializer.Serialize(holidays));
      var service = new HolidayApiService(httpClient, cache.Object);

      // Act
      var result = await service.IsHolidayAsync(date, countryCode, year);

      // Assert
      Assert.IsFalse(result);
    }
  }
}
