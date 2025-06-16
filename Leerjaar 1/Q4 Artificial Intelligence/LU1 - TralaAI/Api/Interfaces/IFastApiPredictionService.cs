using Api.Models;

namespace Api.Interfaces
{
    public interface IFastApiPredictionService
    {
        Task<PredictionResponseWrapper> MakeLitterAmountPredictionAsync(List<PredictionRequestModel> requestModels);
    }

    // [Obsolete("Retraining the model is not supported in the current version. This method will be added in a future release.")]
    // Task<bool> RetrainModelAsync();
}