using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services;
using Api.Interfaces;
using Api.Repository;
using Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// 🪄 Add custom variables for later use.
var holidayApiKey = builder.Configuration.GetSection("ApiKeys")["HolidayApiKey"];
if (string.IsNullOrWhiteSpace(holidayApiKey))
    throw new InvalidOperationException("Holiday API key is not configured. Please set the 'HolidayApiKey' in the user secrets.");

var sensoringApiKey = builder.Configuration.GetSection("ApiKeys")["SensoringApiKey"];
if (string.IsNullOrWhiteSpace(sensoringApiKey))
    throw new InvalidOperationException("Sensoring API key is not configured. Please set the 'SenoringApiKey' in the user secrets.");

// 🛠️ Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddDbContext<LitterDbContext>(options => options.UseSqlServer(builder.Configuration.GetSection("Database")["ConnectionString"]));
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IHolidayApiService, HolidayApiService>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new HolidayApiService(holidayApiKey, httpClient);
});
builder.Services.AddScoped<IAggregatedTrashService, AggregatedTrashService>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
    return new AggregatedTrashService(httpClient, sensoringApiKey);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
