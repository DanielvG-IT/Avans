using Api;
using Api.Data;
using Api.Services;
using Api.Interfaces;
using Api.Repository;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Credential management
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

else
    builder.Configuration.AddEnvironmentVariables();

// Configure and validate options
builder.Services.AddOptions<ApiKeysOptions>()
    .Bind(builder.Configuration.GetSection(ApiKeysOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<ApiSettingsOptions>()
    .Bind(builder.Configuration.GetSection(ApiSettingsOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// 🛠️ Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<LitterDbContext>(options => options.UseSqlServer(builder.Configuration.GetSection("Database")["ConnectionString"]));
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IDTOService, DTOService>();

builder.Services.AddHttpClient<IFastApiPredictionService, FastApiPredictionService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
    var apiKeys = serviceProvider.GetRequiredService<IOptions<ApiKeysOptions>>().Value;
    client.DefaultRequestHeaders.Add("X-API-Key", apiKeys.FastApiKey);
    client.BaseAddress = new Uri(apiSettings.FastApiBaseAddress);
});

builder.Services.AddHttpClient<IHolidayApiService, HolidayApiService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
    client.BaseAddress = new Uri(apiSettings.HolidayApiBaseAddress);
    var apiKeys = serviceProvider.GetRequiredService<IOptions<ApiKeysOptions>>().Value;
});

builder.Services.AddHttpClient<ITrashImportService, TrashImportService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
    var apiKeys = serviceProvider.GetRequiredService<IOptions<ApiKeysOptions>>().Value;
    client.BaseAddress = new Uri(apiSettings.SensoringApiBaseAddress);
    client.DefaultRequestHeaders.Add("X-API-KEY", apiKeys.SensoringApiKey);
});

builder.Services.AddHttpClient<IWeatherService, WeatherService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
    var apiKeys = serviceProvider.GetRequiredService<IOptions<ApiKeysOptions>>().Value;
    client.BaseAddress = new Uri(apiSettings.WeatherApiBaseAddress);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();