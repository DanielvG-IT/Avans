using Blazor.Models;
using Blazor.Models.Health;

namespace Blazor.Interfaces;

/// <summary>
/// Defines the contract for a service that interacts with the backend API.
/// </summary>
public interface IBackendService
{
  /// <summary>
  /// Asynchronously retrieves a list of litters based on an optional filter.
  /// </summary>
  /// <param name="filter">The data transfer object containing filter criteria. Can be null.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Litter"/> objects, or null if the request fails.</returns>
  Task<List<Litter>?> GetLittersAsync(LitterFilterDto? filter);

  /// <summary>
  /// Asynchronously requests predictions from the backend.
  /// </summary>
  /// <param name="amountOfDays">The number of days to predict into the future.</param>
  /// <param name="location">The location for which to generate predictions.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="PredictionResponse"/> objects, or null if the request fails.</returns>
  Task<List<PredictionResponse>?> PredictAsync(int amountOfDays, string location);

  /// <summary>
  /// Asynchronously sends a request to retrain the machine learning model on the backend.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains a string with a status message from the backend, or null if the request fails.</returns>
  Task<string?> RetrainModelAsync();

  /// <summary>
  /// Asynchronously retrieves the health status of the backend service.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="HealthStatus"/> of the backend, or null if the request fails.</returns>
  Task<HealthStatus?> GetHealthStatusAsync();
}