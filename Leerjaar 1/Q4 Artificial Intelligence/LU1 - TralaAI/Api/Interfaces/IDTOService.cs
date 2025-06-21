using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Interfaces;

/// <summary>
/// Provides methods for converting litter and weather data to standardized categories.
/// </summary>
public interface IDTOService
{
    /// <summary>
    /// Gets the <see cref="LitterCategory"/> associated with a given <see cref="LitterType"/>.
    /// </summary>
    /// <param name="litter">The litter type to classify.</param>
    /// <returns>The corresponding <see cref="LitterCategory"/>, or null if input is null.</returns>
    LitterCategory? GetCategory(LitterType? litter);

    /// <summary>
    /// Parses a string into <see cref="LitterType"/> and returns its corresponding <see cref="LitterCategory"/>.
    /// </summary>
    /// <param name="litter">The litter type string to parse and classify.</param>
    /// <returns>The corresponding <see cref="LitterCategory"/>, or null if input is null or whitespace, or <see cref="LitterCategory.Unknown"/> if parsing fails.</returns>
    LitterCategory? GetCategory(string? litter);

    /// <summary>
    /// Gets the <see cref="WeatherCategory"/> associated with a given <see cref="WeatherCondition"/>.
    /// </summary>
    /// <param name="weather">The weather condition to classify.</param>
    /// <returns>The corresponding <see cref="WeatherCategory"/>, or null if input is null.</returns>
    WeatherCategory? GetWeatherCategory(WeatherCondition? weather);

    /// <summary>
    /// Parses a string into <see cref="WeatherCondition"/> and returns its corresponding <see cref="WeatherCategory"/>.
    /// </summary>
    /// <param name="weather">The weather condition string to parse and classify.</param>
    /// <returns>The corresponding <see cref="WeatherCategory"/>, or null if input is null or whitespace, or <see cref="WeatherCategory.Unknown"/> if parsing fails.</returns>
    WeatherCategory? GetWeatherCategory(string? weather);

    /// <summary>
    /// Gets an index value for the given <see cref="WeatherCategory"/>.
    /// </summary>
    /// <param name="weatherCategory">The weather category.</param>
    /// <returns>An integer index (1–6) or null if the category is null or unknown.</returns>
    int? GetWeatherCategoryIndex(WeatherCategory? weatherCategory);
}