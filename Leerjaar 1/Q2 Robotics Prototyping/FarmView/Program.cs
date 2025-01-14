using FarmView.Components;
using SimpleMqtt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Register DatabaseAccess component with Connection String
var connectionString = "Server=aei-sql2.avans.nl,1443;Database=DB2226789;UID=ITI2226789;password=H7lcQ2F0;TrustServerCertificate=true;";
builder.Services.AddSingleton<IDatabaseAccess, DatabaseAccess>(o => new DatabaseAccess(connectionString));

// Register SimpleMqttClientConfiguration with options
var simpleMqttClient = new SimpleMqttClient(new()
{
    Host = "82460450721346f1b2b5f164a15671c9.s1.eu.hivemq.cloud",
    Port = 8883,
    ClientId = "FarmView",
    TimeoutInMs = 10_000,
    UserName = "hivemq.webclient.1732899035765",
    Password = "P107L9Dq;yYuMh>ceV#:",
    CleanStart = true
});
builder.Services.AddSingleton(simpleMqttClient);

// Register MqttProcessingService
builder.Services.AddSingleton<MqttProcessingService>();
builder.Services.AddSingleton<IMqttProcessingService>(provider => provider.GetRequiredService<MqttProcessingService>());
builder.Services.AddHostedService(provider => provider.GetRequiredService<MqttProcessingService>());

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
