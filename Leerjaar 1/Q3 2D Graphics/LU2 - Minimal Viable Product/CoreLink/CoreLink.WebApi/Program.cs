using CoreLink.WebApi.Services;
using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var sqlConnectionString = builder.Configuration["SqlConnectionString"] ?? string.Empty;
var sqlConnectionStringFound = !string.IsNullOrEmpty(sqlConnectionString);

// Configure Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddDapperStores(options => options.ConnectionString = sqlConnectionString);

// Configure CORS to allow WebGL build
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCoreGame",
        policy =>
        {
            policy.WithOrigins("https://coregame.danielvanginneken.nl")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Register services
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();
builder.Services.AddTransient<IObjectRepository, ObjectRepository>(_ => new ObjectRepository(sqlConnectionString));
builder.Services.AddTransient<IEnvironmentRepository, EnvironmentRepository>(_ => new EnvironmentRepository(sqlConnectionString));

var app = builder.Build();

// Enable OpenApi in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", (Microsoft.Extensions.Options.IOptions<IdentityOptions> identityOptions) =>
{
    var environment = app.Environment.EnvironmentName;
    var connectionStringStatus = sqlConnectionStringFound ? "✅ Found" : "❌ Not Found";
    var passwordOptions = identityOptions.Value.Password;
    var passwordRequirements = $@"
        <ul>
            <li><strong>Required Length:</strong> {passwordOptions.RequiredLength}</li>
            <li><strong>Require Digit:</strong> {passwordOptions.RequireDigit}</li>
            <li><strong>Require Lowercase:</strong> {passwordOptions.RequireLowercase}</li>
            <li><strong>Require Uppercase:</strong> {passwordOptions.RequireUppercase}</li>
            <li><strong>Require Non-Alphanumeric:</strong> {passwordOptions.RequireNonAlphanumeric}</li>
        </ul>";

    var buildDate = System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location);
    var deployDate = DateTime.UtcNow; // Assuming current time as deploy time

    var additionalInfo = new
    {
        ApplicationName = "CoreLink Web API",
        Version = "1.0.4",
        DeveloperContact = "dcj.vanginneken@student.avans.nl",
        DocumentationLink = "https://github.com/DanielvG-IT/Avans/blob/main/Leerjaar%201/Q3%202D%20Graphics/LU2%20-%20Minimal%20Viable%20Product/docs/Design/APIEndpoints.md"
    };

    var html = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>CoreLink Web API - Status</title>
            <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css'>
        </head>
        <body class='bg-light'>
            <div class='container mt-5'>
                <div class='card shadow-sm'>
                    <div class='card-header bg-primary text-white'>
                        <h2>CoreLink Web API - Status</h2>
                    </div>
                    <div class='card-body'>
                        <p><strong>Environment:</strong> {environment}</p>
                        <p><strong>Connection String:</strong> {connectionStringStatus}</p>
                        <h4>Password Policy:</h4>
                        {passwordRequirements}
                        <p><strong>Build Date:</strong> {buildDate.ToString("yyyy-MM-dd HH:mm:ss")}</p>
                        <p><strong>Deploy Date:</strong> {deployDate.ToString("yyyy-MM-dd HH:mm:ss")} (UTC)</p>
                        <h4>Additional Info:</h4>
                        <p><strong>Application:</strong> {additionalInfo.ApplicationName}</p>
                        <p><strong>Version:</strong> {additionalInfo.Version}</p>
                        <p><strong>Developer Contact:</strong> <a href='mailto:{additionalInfo.DeveloperContact}'>{additionalInfo.DeveloperContact}</a></p>
                        <p><strong>Documentation:</strong> <a href='{additionalInfo.DocumentationLink}' target='_blank'>View Docs</a></p>
                    </div>
                </div>
            </div>
        </body>
        </html>";

    return Results.Text(html, "text/html");
});


app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();
app.UseCors("AllowCoreGame");
app.Run();
