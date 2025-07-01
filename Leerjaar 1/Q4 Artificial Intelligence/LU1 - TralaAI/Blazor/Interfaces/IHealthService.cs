using Api.Models;

namespace Blazor.Interfaces;

/// <summary>
/// Defines methods to check the health status of various backend services and APIs.
/// </summary>
public interface IHealthService
{
    /// <summary>
    /// Checks the health status of the main backend service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the backend is healthy; otherwise, <c>false</c>.</returns>
    Task<bool> GetBackendHealthAsync();

    /// <summary>
    /// Checks the health status of the FastAPI service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the FastAPI service is healthy; otherwise, <c>false</c>.</returns>
    Task<bool> GetFastApiHealthAsync();

    /// <summary>
    /// Checks the health status of the sensoring service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the sensoring service is healthy; otherwise, <c>false</c>.</returns>
    Task<bool> GetSensoringHealthAsync();

    /// <summary>
    /// Checks the health status of the Holiday API service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the Holiday API service is healthy; otherwise, <c>false</c>.</returns>
    Task<bool> GetHolidayApiHealthAsync();

    /// <summary>
    /// Checks the health status of the Weather API service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the Weather API service is healthy; otherwise, <c>false</c>.</returns>
    Task<bool> GetWeatherApiHealthAsync();

    /// <summary>
    /// Asynchronously retrieves model data from the FastAPI service for the specified camera.
    /// </summary>
    /// <param name="cameraId">The unique identifier of the camera for which to retrieve model data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="ModelStatusResponse"/> object with the model data,
    /// or <c>null</c> if no data is available.
    /// </returns>
    Task<ModelStatusResponse?> GetFastApiModelDataAsync(int cameraId);

}