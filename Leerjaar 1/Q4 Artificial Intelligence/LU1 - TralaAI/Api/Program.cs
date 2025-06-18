using Api;
using Api.Data;
using Api.Services;
using Api.Interfaces;
using Api.Repository;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Credential management
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();
else
    builder.Configuration.AddAzureKeyVault(new Uri(Environment.GetEnvironmentVariable("KEY_VAULT_URI") ?? throw new InvalidOperationException("KEY_VAULT_URI is not set.")), new DefaultAzureCredential());

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

builder.Services.AddDbContext<LitterDbContext>(options => options.UseSqlServer(builder.Configuration.GetSection("Database")["ConnectionString"]));
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

builder.Services.AddHttpClient<IFastApiPredictionService, FastApiPredictionService>((serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettingsOptions>>().Value;
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
    client.DefaultRequestHeaders.Add("API-Key", apiKeys.SensoringApiKey);
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