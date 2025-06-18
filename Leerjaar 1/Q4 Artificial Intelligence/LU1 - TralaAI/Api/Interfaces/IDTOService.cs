using Api.Models.Enums;
using Api.Models.Enums.DTO;

namespace Api.Interfaces;

/// <summary>
/// Defines a service contract for operations related to Data Transfer Objects (DTOs).
/// This service may be responsible for tasks such as mapping domain entities to DTOs,
/// or providing DTO-specific lookups and transformations.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to handle the logic
/// for converting or accessing data in a DTO-friendly manner, facilitating
/// interaction between different layers or components of an application.
/// </remarks>
public interface IDTOService
{
    /// <summary>
    /// Retrieves the <see cref="LitterCategory"/> associated with a specific <see cref="LitterType"/>.
    /// </summary>
    /// <param name="litter">The type of litter for which to retrieve the category.
    /// This value can be <c>null</c>.</param>
    /// <returns>
    /// The <see cref="LitterCategory"/> corresponding to the provided <paramref name="litter"/>.
    /// Returns <c>null</c> if the <paramref name="litter"/> is <c>null</c>,
    /// or if no category is defined for the given litter type.
    /// </returns>
    LitterCategory? GetCategory(LitterType? litter);

    /// <summary>
    /// Determines the weather category based on the provided weather condition.
    /// </summary>
    /// <param name="weather">The weather condition to evaluate. Can be null.</param>
    /// <returns>
    /// A <see cref="WeatherCategory"/> representing the category of the weather condition,
    /// or null if the input weather condition is null or cannot be categorized.
    /// </returns>
    public WeatherCategory? GetWeatherCategory(WeatherCondition? weather);
}