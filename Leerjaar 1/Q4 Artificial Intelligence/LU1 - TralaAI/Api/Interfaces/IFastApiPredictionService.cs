using Api.Services;

namespace Api.Interfaces
{
    public interface IFastApiPredictionService
    {
        Task<PredictionResponse?> MakeLitterAmountPredictionAsync(DateTime date);

        Task<bool> RetrainModelAsync();

    }
}