using Blazor.Components;
using Blazor.Interfaces;
using Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<IBackendService, BackendService>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//-------------------------------HIER NOG DE URL VOOR DE BACKEND API--------------------------
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://your-api-url.com/") });


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
