using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Interfaces;

/// <summary>
/// Provides methods to categorize litter and weather conditions.
/// </summary>
public interface IDTOService
{
    /// <summary>
    /// Determines the category of a given litter type.
    /// </summary>
    /// <param name="litter">The litter type to categorize.</param>
    /// <returns>The category of the litter type, or null if the input is null or cannot be categorized.</returns>
    LitterCategory? GetCategory(LitterType? litter);

    /// <summary>
    /// Determines the category of a given litter based on its string representation.
    /// </summary>
    /// <param name="litter">The string representation of the litter to categorize.</param>
    /// <returns>The category of the litter, or null if the input is null or cannot be categorized.</returns>
    LitterCategory? GetCategory(string? litter);

    /// <summary>
    /// Determines the weather category for a given weather condition.
    /// </summary>
    /// <param name="weather">The weather condition to categorize.</param>
    /// <returns>The weather category, or null if the input is null or cannot be categorized.</returns>
    WeatherCategory? GetWeatherCategory(WeatherCondition? weather);

    /// <summary>
    /// Determines the weather category for a given weather condition based on its string representation.
    /// </summary>
    /// <param name="weather">The string representation of the weather condition to categorize.</param>
    /// <returns>The weather category, or null if the input is null or cannot be categorized.</returns>
    WeatherCategory? GetWeatherCategory(string? weather);
}