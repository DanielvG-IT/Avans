using Api.Models.Enums;
using Api.Models.Data;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Services
{
    public class TrashImportService(ILitterRepository litterRepository, IHolidayApiService holidayApiService, IDTOService dTOService, HttpClient httpClient) : ITrashImportService
    {
        private readonly ILitterRepository _litterRepository = litterRepository;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;
        private readonly IDTOService _dTOService = dTOService;
        private readonly HttpClient _httpClient = httpClient;

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
            // Get the latest litter timestamp for cameraId 2 (sensoring group)
            DateTime latestLitter;
            try
            {
                latestLitter = await _litterRepository.GetLatestLitterTimeAsync(2) ?? new DateTime(1970, 1, 1);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to get latest litter time: {ex.Message}");
                return false;
            }

            // Prepare the API request to fetch new litter data
            var requestUrl = $"/Litter/getLitter?dateTime1={latestLitter:yyyy-MM-ddTHH:mm:ss}&dateTime2={DateTime.Now:yyyy-MM-ddTHH:mm:ss}";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            AggregatedTrashDto? results;
            try
            {
                var response = await _httpClient.SendAsync(request, ct);
                response.EnsureSuccessStatusCode();
                results = await response.Content.ReadFromJsonAsync<AggregatedTrashDto>(cancellationToken: ct);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"HTTP request failed: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to fetch or parse litter data: {ex.Message}");
                return false;
            }

            // Handle the case where no data is returned
            if (results is null || results.Litters is null || results.Litters.Count == 0)
            {
                Console.WriteLine("Couldn't import sensoring data because we received NULL or empty results.");
                // TODO @SanderBosselaar: Consider adding more robust error handling here.
                return false;
            }

            // Get all unique dates from the results
            var uniqueDates = results.Litters.Select(t => t.Date.Date).Distinct().ToList();

            // Get holiday information for all dates
            var holidayDictionary = new Dictionary<DateOnly, bool>();
            foreach (var date in uniqueDates)
            {
                try
                {
                    holidayDictionary[DateOnly.FromDateTime(date)] =
                    await _holidayApiService.IsHolidayAsync(date, "NL", date.Year.ToString()) ?? false;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to get holiday info for {date:yyyy-MM-dd}: {ex.Message}");
                    holidayDictionary[DateOnly.FromDateTime(date)] = false;
                }
            }

            var newLitters = new List<Litter>();

            foreach (var trash in results.Litters)
            {
                try
                {
                    // Determine if it's a holiday and get the category
                    var isHoliday = holidayDictionary[DateOnly.FromDateTime(trash.Date.Date)];
                    var switchedType = trash.Type is not null ? _dTOService.GetCategory(trash.Type) : LitterCategory.Unknown;

                    // Create a new Litter object
                    var litter = new Litter
                    {
                        LitterCategory = switchedType ?? LitterCategory.Unknown,
                        TimeStamp = trash.Date,
                        Confidence = trash.Confidence,
                        WeatherCategory = _dTOService.GetWeatherCategory(trash.Weather) ?? WeatherCategory.Unknown,
                        Temperature = (int)trash.Temperature,
                        IsHoliday = isHoliday,
                        CameraId = 2 // Assuming a sensoring group camera ID
                    };

                    newLitters.Add(litter);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to process trash item with Id {trash.Id}: {ex.Message}");
                }
            }

            try
            {
                // Use batch operation for efficiency
                return await _litterRepository.AddAsync(newLitters);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to add new litters to repository: {ex.Message}");
                return false;
            }
        }
    }
}
