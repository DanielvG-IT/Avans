using Api.Models;

namespace Api.Interfaces;

public interface IWeatherService
{
    Task<FastApiWeatherRequirements> GetWeatherAsync(DateOnly date);
}