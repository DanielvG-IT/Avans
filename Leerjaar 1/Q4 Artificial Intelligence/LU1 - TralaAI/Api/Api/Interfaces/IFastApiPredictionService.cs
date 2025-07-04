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
        /// <param name="requestModels">A list of <see cref="PredictionRequest"/>, each representing a set of features for a single prediction.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="PredictionResponse"/>
        /// which includes the prediction results.
        /// </returns>
        Task<List<PredictionResponse>> MakeLitterAmountPredictionAsync(PredictionRequest requestModels);


        /// <summary>
        /// Asynchronously retrains the prediction model.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean indicating
        /// whether the retraining process was initiated successfully (true) or not (false).
        /// </returns>
        Task<bool> RetrainModelAsync(int CameraId);

        /// <summary>
        /// Checks the status of the Fast API service.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean indicating
        /// whether the Fast API service is healthy (true) or not (false).
        /// </returns>
        Task<bool> GetStatusAsync();

        /// <summary>
        /// Retrieves the status of the model associated with the specified camera.
        /// </summary>
        /// <param name="cameraId">The unique identifier of the camera.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a <see cref="ModelStatusResponse"/> 
        /// object with the model status information, or <c>null</c> if the status could not be retrieved.
        /// </returns>
        Task<ModelStatusResponse?> GetModelStatus(int cameraId);
    }
}