using Api.Models.Enums;
using Api.Models.Data;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Services
{
    public class TrashImportService : ITrashImportService
    {
        private readonly ILitterRepository _litterRepository;
        private readonly IHolidayApiService _holidayApiService;
        private readonly IDTOService _dTOService;
        private readonly HttpClient _httpClient;

        public TrashImportService(
            ILitterRepository litterRepository,
            IHolidayApiService holidayApiService,
            IDTOService dTOService,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            // Check for required configuration keys
            var apiKeysSection = configuration.GetSection("apiKeys");
            var apiSettingsSection = configuration.GetSection("apiSettings");

            if (!apiKeysSection.Exists())
                throw new InvalidOperationException("Missing required configuration section: apiKeys");
            if (!apiSettingsSection.Exists())
                throw new InvalidOperationException("Missing required configuration section: apiSettings");

            _litterRepository = litterRepository;
            _holidayApiService = holidayApiService;
            _dTOService = dTOService;
            _httpClient = httpClient;
        }

        public async Task<bool> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ImportAsync(CancellationToken ct)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Litter/getLitter");
                var response = await _httpClient.SendAsync(request, ct);
                var results = await response.Content.ReadFromJsonAsync<List<AggregatedTrashDto>>(cancellationToken: ct);
                if (results is null || results.Count == 0)
                {
                    Console.WriteLine("Couldn't import sensoring data because we received NULL"); // TODO @SanderBosselaar Maybe add more error handeling?
                    return false;
                }

                // Get all unique dates from the results
                var uniqueDates = results.Select(t => t.Date.Date).Distinct().ToList();

                // Get holiday information for all dates
                var holidayDictionary = new Dictionary<DateOnly, bool>();
                foreach (var date in uniqueDates)
                    holidayDictionary[DateOnly.FromDateTime(date)] = await _holidayApiService.IsHolidayAsync(date, "NL", date.Year.ToString()) ?? false;

                var newLitters = new List<Litter>();

                foreach (var trash in results)
                {
                    // Determine if it's a holiday and get the category
                    var isHoliday = holidayDictionary[DateOnly.FromDateTime(trash.Date.Date)];
                    var switchedType = trash.Type is not null ? _dTOService.GetCategory(trash.Type) : LitterCategory.Unknown;

                    // Create a new Litter object
                    var litter = new Litter
                    {
                        Id = trash.Id,
                        LitterCategory = switchedType ?? LitterCategory.Unknown,
                        TimeStamp = trash.Date,
                        Confidence = trash.Confidence,
                        WeatherCategory = _dTOService.GetWeatherCategory(trash.Weather) ?? WeatherCategory.Unknown,
                        Temperature = trash.Temperature,
                        IsHoliday = isHoliday,
                        CameraId = 2 // Assuming a sensoring group camera ID
                    };

                    newLitters.Add(litter);
                }

                // Use batch operation for efficiency
                return await _litterRepository.AddAsync(newLitters);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in ImportAsync: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
