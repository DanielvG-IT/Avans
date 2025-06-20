namespace Api.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that provides holiday information.
    /// </summary>
    public interface IHolidayApiService
    {
        /// <summary>
        /// Asynchronously checks if a given date is a public holiday in a specified country.
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <param name="countryCode">The ISO 3166-1 alpha-2 country code (e.g., "US", "GB").</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains <c>true</c> if the specified date is a public holiday in the given country; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsHolidayAsync(DateTime date, string countryCode, string year);
    }
}