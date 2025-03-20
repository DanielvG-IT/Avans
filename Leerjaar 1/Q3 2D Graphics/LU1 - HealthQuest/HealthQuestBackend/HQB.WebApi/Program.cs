using System.Text;
using HQB.WebApi.Data;
using HQB.WebApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HealthQuestConnectionString") ?? throw new InvalidOperationException("Connection string 'HealthQuestConnectionString' not found."); ;

// 🔹 Load Configuration
var configuration = builder.Configuration;

// 🔹 Add Database Context (EF Core with SQL Server)
builder.Services.AddDbContext<ZorgAppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("HealthQuestConnectionString")));

// 🔹 Configure Identity
builder.Services.AddDefaultIdentity<OuderVoogd>()
    .AddEntityFrameworkStores<ZorgAppDbContext>()
    .AddDefaultTokenProviders();

// 🔹 Configure Authentication & JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };
    });

// 🔹 Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Enable OpenApi in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

app.MapGet("/", (IOptions<IdentityOptions> identityOptions) =>
{
    var environment = app.Environment.EnvironmentName;
    var connectionStringStatus = string.IsNullOrEmpty(configuration.GetConnectionString("HealthQuestConnectionString")) ? "❌ Not Found" : "✅ Found";
    var passwordOptions = identityOptions.Value.Password;
    var passwordRequirements = $@"
        <ul>
            <li><strong>Required Length:</strong> {passwordOptions.RequiredLength}</li>
            <li><strong>Require Digit:</strong> {passwordOptions.RequireDigit}</li>
            <li><strong>Require Lowercase:</strong> {passwordOptions.RequireLowercase}</li>
            <li><strong>Require Uppercase:</strong> {passwordOptions.RequireUppercase}</li>
            <li><strong>Require Non-Alphanumeric:</strong> {passwordOptions.RequireNonAlphanumeric}</li>
        </ul>";

    var buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

    var additionalInfo = new
    {
        ApplicationName = "HealthQuest Web API",
        Version = "0.2.4",
        DeveloperContact = "dcj.vanginneken@student.avans.nl",
        DocumentationLink = "https://github.com/DanielvG-IT/Avans/blob/main/Leerjaar%201/Q3%202D%20Graphics/"
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
                        <h2>HealthQuest Web API - Status</h2>
                    </div>
                    <div class='card-body'>
                        <p><strong>Environment:</strong> {environment}</p>
                        <p><strong>Connection String:</strong> {connectionStringStatus}</p>
                        <h4>Password Policy:</h4>
                        {passwordRequirements}
                        <p><strong>Build Date:</strong> {buildDate:yyyy-MM-dd HH:mm:ss}</p>
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

app.MapControllers();

app.Run();