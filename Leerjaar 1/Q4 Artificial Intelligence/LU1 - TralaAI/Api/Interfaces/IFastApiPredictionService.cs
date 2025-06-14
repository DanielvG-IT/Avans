using Api.Services;

namespace Api.Interfaces
{
    public interface IFastApiPredictionService
    {
        Task<PredictionResponse?> MakeLitterAmountPredictionAsync(DateTime date);

        [Obsolete("Retraining the model is not supported in the current version. This method will be added in a future release.")]
        Task<bool> RetrainModelAsync();

    }
}