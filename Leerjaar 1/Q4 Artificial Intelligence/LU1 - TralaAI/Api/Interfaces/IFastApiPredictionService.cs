using Api.Models;

/// <summary>
/// Defines the contract for a service that interacts with a Fast API for making predictions.
/// </summary>
namespace Api.Interfaces
{
    /// <summary>
    /// Provides methods for making predictions and potentially retraining models via a Fast API.
    /// </summary>
    public interface IFastApiPredictionService
    {
        /// <summary>
        /// Makes a prediction for the amount of litter based on a list of input features.
        /// </summary>
        /// <param name="requestModels">A list of <see cref="PredictionRequestModel"/>, each representing a set of features for a single prediction.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PredictionResponseWrapper"/>
        /// which includes the prediction results.
        /// </returns>
        Task<PredictionResponseWrapper> MakeLitterAmountPredictionAsync(List<PredictionRequestModel> requestModels);
    }

    /// <summary>
    /// Asynchronously retrains the prediction model.
    /// </summary>
    /// <remarks>
    /// This method is currently obsolete and not supported. It is planned for a future release.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a boolean indicating
    /// whether the retraining process was initiated successfully (true) or not (false).
    /// </returns>
    // [Obsolete("Retraining the model is not supported in the current version. This method will be added in a future release.")]
    // Task<bool> RetrainModelAsync();
}