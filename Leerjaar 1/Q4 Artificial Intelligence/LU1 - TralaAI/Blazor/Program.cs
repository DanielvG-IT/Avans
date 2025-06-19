using Azure.Identity;
using Blazor.Services;
using Blazor.Components;
using Blazor.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Credential management
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

else
    builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddHttpClient<IBackendService, BackendService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var backendApiBaseAddress = configuration["ApiSettings:BackendApiBaseAddress"] ?? throw new InvalidOperationException("ApiSettings:BackendApiBaseAddress configuration is missing.");
    client.BaseAddress = new Uri(backendApiBaseAddress);
    client.DefaultRequestHeaders.Add("X-API-KEY", configuration["ApiKeys:BackendApiKey"] ?? throw new InvalidOperationException("ApiKeys:BackendApiKey configuration is missing."));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
