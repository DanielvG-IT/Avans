using Blazor.Models;

namespace Blazor.Interfaces;

/// <summary>
/// Defines methods for managing and analyzing litter data, including retrieval, prediction, and model retraining operations.
/// </summary>
public interface ILitterService
{
  /// <summary>
  /// Retrieves a list of litters based on the specified filter criteria.
  /// </summary>
  /// <param name="filter">The filter criteria for retrieving litters.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of litters or null.</returns>
  Task<List<Litter>?> GetLittersAsync(LitterFilterDto? filter);

  /// <summary>
  /// Retrieves a list of available cameras.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of cameras or null.</returns>
  Task<List<Camera>?> GetCamerasAsync();

  /// <summary>
  /// Predicts litter occurrences for a specified number of days and camera.
  /// </summary>
  /// <param name="amountOfDays">The number of days to predict.</param>
  /// <param name="cameraId">The identifier of the camera.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of prediction responses or null.</returns>
  Task<List<PredictionResponse>?> PredictAsync(int amountOfDays, int cameraId);

  /// <summary>
  /// Retrains the prediction model for a specific camera.
  /// </summary>
  /// <param name="cameraId">The identifier of the camera.</param>
  /// <returns>A task that represents the asynchronous operation. The task result indicates whether the retraining was successful.</returns>
  Task<bool> RetrainModelAsync(int cameraId);

  /// <summary>
  /// Imports trash data asynchronously.
  /// </summary>
  /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains an import result message or null.</returns>
  Task<string?> ImportTrashDataAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Retrieves the latest litters, limited by the specified amount.
  /// </summary>
  /// <param name="amount">The maximum number of latest litters to retrieve.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a list of the latest litters or null.</returns>
  Task<List<Litter>?> GetLatestLittersAsync(int? amount);

  /// <summary>
  /// Retrieves the amount of each litter type per location.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains the amount per location or null.</returns>
  Task<LitterTypeAmount?> GetAmountPerLocationAsync();

  /// <summary>
  /// Retrieves the history of litter data.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation. The task result contains the litter history response or null.</returns>
  Task<LitterHistoryResponse?> GetLitterHistoryAsync();
}