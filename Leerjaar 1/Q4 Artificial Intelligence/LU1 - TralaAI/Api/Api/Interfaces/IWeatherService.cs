using Api.Models;

namespace Api.Interfaces;

public interface IWeatherService
{
    /// <summary>
    /// Fetches weather data for a specified number of days.
    /// </summary>
    /// <param name="amountOfDays">The number of days to fetch weather data for (1-14).</param>
    /// <returns>A list of weather requirements for the specified number of days.</returns>
    /// <exception cref="ArgumentException">Thrown if the amountOfDays is not between 1 and 14.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    Task<List<FastApiWeatherRequirements>> GetWeatherAsync(int amountOfDays);

    /// <summary>
    /// Asynchronously retrieves the current status of the weather service.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains <c>true</c> if the service is operational; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> GetStatusAsync();
}