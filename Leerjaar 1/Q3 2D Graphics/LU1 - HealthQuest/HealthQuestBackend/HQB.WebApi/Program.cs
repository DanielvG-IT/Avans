using System.Reflection;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Add Identity Framework services
var sqlConnectionString = builder.Configuration["SQLConnection"];
if (string.IsNullOrEmpty(sqlConnectionString))
{
    throw new InvalidOperationException("Connection string 'SQLConnection' is not configured.");
}

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddRoles<IdentityRole>().AddDapperStores(options => options.ConnectionString = sqlConnectionString);
builder.Services.AddTransient<IGuardianRepository, GuardianRepository>(_ => new GuardianRepository(sqlConnectionString));
builder.Services.AddTransient<IPatientRepository, PatientRepository>(_ => new PatientRepository(sqlConnectionString));
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapOpenApi();

app.MapGet("/", (IOptions<IdentityOptions> identityOptions, IWebHostEnvironment env) =>
{
    var passwordOptions = identityOptions.Value.Password;
    var buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

    string html = $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>CoreLink Web API - Status</title>
            <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css'>
            <style>
                body {{ background-color: #121212; color: #ffffff; font-family: Arial, sans-serif; }}
                .container {{ max-width: 800px; margin-top: 50px; }}
                .card {{ border-radius: 12px; background: #1e1e1e; box-shadow: 0 8px 16px rgba(0,0,0,0.2); }}
                .card-header {{ font-size: 1.8rem; font-weight: bold; background: linear-gradient(135deg, #007bff, #00c6ff); color: white; text-align: center; padding: 15px; }}
                .list-group-item {{ font-size: 1.2rem; background: #1e1e1e; border-color: #333; color: #ffffff; }}
                a {{ text-decoration: none; color: #00c6ff; font-weight: bold; }}
                a:hover {{ text-decoration: underline; color: #ffffff; }}
                .glow {{ text-shadow: 0 0 8px #00c6ff; }}
                .fade-in {{ animation: fadeIn 1s ease-in-out; }}
                @keyframes fadeIn {{ from {{ opacity: 0; }} to {{ opacity: 1; }} }}
            </style>
        </head>
        <body>
            <div class='container fade-in'>
                <div class='card'>
                    <div class='card-header glow'>CoreLink Web API - Status</div>
                    <div class='card-body'>
                        <ul class='list-group list-group-flush'>
                            <li class='list-group-item'><strong>Environment:</strong> {env.EnvironmentName}</li>
                            <li class='list-group-item'><strong>Connection String:</strong> {(string.IsNullOrEmpty(sqlConnectionString) ? "❌ Not Found" : "✅ Found")}</li>
                            <li class='list-group-item'><strong>Build Date:</strong> {buildDate:yyyy-MM-dd HH:mm:ss}</li>
                        </ul>
                        <h4 class='mt-4 glow'>Password Policy</h4>
                        <ul class='list-group list-group-flush'>
                            <li class='list-group-item'><strong>Required Length:</strong> {passwordOptions.RequiredLength}</li>
                            <li class='list-group-item'><strong>Require Digit:</strong> {passwordOptions.RequireDigit}</li>
                            <li class='list-group-item'><strong>Require Lowercase:</strong> {passwordOptions.RequireLowercase}</li>
                            <li class='list-group-item'><strong>Require Uppercase:</strong> {passwordOptions.RequireUppercase}</li>
                            <li class='list-group-item'><strong>Require Non-Alphanumeric:</strong> {passwordOptions.RequireNonAlphanumeric}</li>
                        </ul>
                        <h4 class='mt-4 glow'>Additional Info</h4>
                        <ul class='list-group list-group-flush'>
                            <li class='list-group-item'><strong>Application:</strong> CoreLink Web API</li>
                            <li class='list-group-item'><strong>Version:</strong> 1.0.4</li>
                            <li class='list-group-item'><strong>Developer Contact:</strong> <a href='mailto:dcj.vanginneken@student.avans.nl'>dcj.vanginneken@student.avans.nl</a></li>
                            <li class='list-group-item'><strong>Documentation:</strong> <a href='https://github.com/DanielvG-IT/Avans/blob/main/Leerjaar%201/Q3%202D%20Graphics/LU2%20-%20Minimal%20Viable%20Product/docs/Design/APIEndpoints.md' target='_blank'>View Docs</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </body>
        </html>";

    return Results.Text(html, "text/html");
});

app.Run();