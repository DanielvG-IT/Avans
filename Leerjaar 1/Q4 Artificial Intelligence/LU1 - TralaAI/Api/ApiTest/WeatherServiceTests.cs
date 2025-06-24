using Microsoft.Extensions.Options;
using System.Text.Json;
using Api.Services;
using System.Net;
using Api.Models;
using Api;

// Define ApiKeysOptions if not already defined elsewhere
namespace ApiTest
{
  // A simple HttpMessageHandler for injecting canned HttpResponseMessage
  public class FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) : HttpMessageHandler
  {
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc = handlerFunc;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return _handlerFunc(request, cancellationToken);
    }
  }

  [TestClass]
  public class WeatherServiceTests
  {
    private const string DummyKey = "dummy-key";
    private static readonly JsonSerializerOptions CamelCaseOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static WeatherService CreateService(Func<HttpRequestMessage, Task<HttpResponseMessage>> handlerFunc)
    {
      var httpClient = new HttpClient(new FakeHttpMessageHandler((req, ct) => handlerFunc(req)))
      {
        BaseAddress = new Uri("https://api.weatherapi.com")
      };
      var options = Options.Create(new ApiKeysOptions { WeatherApiKey = DummyKey });
      return new WeatherService(httpClient, options);
    }

    [TestMethod]
    public async Task GetStatusAsync_ReturnsTrue_OnSuccessStatusCode()
    {
      var service = CreateService(req => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
      var result = await service.GetStatusAsync();

      Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task GetStatusAsync_ReturnsFalse_OnNonSuccessStatusCode()
    {
      var service = CreateService(req => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
      var result = await service.GetStatusAsync();

      Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetStatusAsync_ReturnsFalse_OnException()
    {
      var service = CreateService(req => throw new HttpRequestException("Network error"));
      var result = await service.GetStatusAsync();

      Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetWeatherAsync_Throws_OnDaysLessThan1()
    {
      var service = CreateService(_ =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

      await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
      {
        await service.GetWeatherAsync(0);
      });
    }

    [TestMethod]
    public async Task GetWeatherAsync_Throws_OnDaysGreaterThan14()
    {
      var service = CreateService(_ =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

      await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
      {
        await service.GetWeatherAsync(15);
      });
    }

    [TestMethod]
    public async Task GetWeatherAsync_Throws_OnNonSuccessResponse()
    {
      var service = CreateService(req =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = "Bad Request"
        }));

      await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
      {
        await service.GetWeatherAsync(3);
      });
    }

    [TestMethod]
    public async Task GetWeatherAsync_Throws_OnMissingForecastData()
    {
      // Return valid 200 but empty or null JSON
      var emptyJson = JsonSerializer.Serialize(new { location = new { }, forecast = new { forecastday = new List<object>() } });
      var service = CreateService(req =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = new StringContent(emptyJson)
        }));

      await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
      {
        await service.GetWeatherAsync(2);
      });
    }

    [TestMethod]
    public async Task GetWeatherAsync_ReturnsForecastList_OnValidResponse()
    {
      // Build a minimal valid JSON response
      var forecastday = new List<ForecastDay>
      {
          new() {
            Date = "2025-06-24",
            Day = new Day
              {
                Condition = new Condition { Text = "Sunny", Code = 1000 },
                AvgtempC = 25.5m
              }
          },
          new() {
            Date = "2025-06-25",
            Day = new Day
              {
              Condition = new Condition { Text = "Rain", Code = 1183 },
              AvgtempC = 18.0m
            }
          }
      };
      var payload = new WeatherResponse
      {
        Location = new Location { Name = "Test City" },
        Forecast = new Forecast { Forecastday = forecastday }
      };

      var json = JsonSerializer.Serialize(payload, CamelCaseOptions);

      var service = CreateService(req =>
          Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
          {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
          }));

      var result = await service.GetWeatherAsync(2);

      Assert.AreEqual(2, result.Count);
      Assert.AreEqual(DateTime.Parse("2025-06-24"), result[0].Date);
      Assert.AreEqual("Sunny", result[0].Condition);
      Assert.AreEqual(1000, result[0].ConditionCode);
      Assert.AreEqual(25.5, result[0].Temperature, 0.01);

      Assert.AreEqual(DateTime.Parse("2025-06-25"), result[1].Date);
      Assert.AreEqual("Rain", result[1].Condition);
      Assert.AreEqual(1183, result[1].ConditionCode);
      Assert.AreEqual(18.0, result[1].Temperature, 0.01);
    }

    [TestMethod]
    public async Task GetWeatherAsync_UsesCache_OnSubsequentCalls()
    {
      int callCount = 0;
      var service = CreateService(req =>
      {
        callCount++;
        // Return simple valid JSON with one day and include location property
        var forecastday = new List<object>
        {
          new
          {
            date = "2025-06-24",
            day = new
            {
              condition = new { text = "Cloudy", code = 1003 },
              avgtemp_c = 20.0
            }
          }
        };
        var payload = new
        {
          location = new { name = "Test City" },
          forecast = new { forecastday }
        };
        var json = JsonSerializer.Serialize(payload);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
      });

      var first = await service.GetWeatherAsync(1);
      var second = await service.GetWeatherAsync(1);

      Assert.AreEqual(1, callCount, "HTTP call should be made only once due to caching");
      Assert.AreEqual(1, first.Count);
      Assert.AreEqual(1, second.Count);
      Assert.AreEqual(first[0].Condition, second[0].Condition);
    }
  }
}
