using FarmView.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register SimpleMqttClientConfiguration with options
builder.Services.AddScoped(provider =>
{
    var config = new SimpleMqtt.SimpleMqttClientConfiguration
    {
        ClientId = "blazor",
        Host = "82460450721346f1b2b5f164a15671c9.s1.eu.hivemq.cloud",
        Port = 8883,
        UserName = "hivemq.webclient.1732899035765",
        Password = "P107L9Dq;yYuMh>ceV#:"
    };
    return config;
});
builder.Services.AddScoped<SimpleMqtt.SimpleMqttClient>();
builder.Services.AddScoped<SimpleMqtt.SimpleMqttMessage>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
