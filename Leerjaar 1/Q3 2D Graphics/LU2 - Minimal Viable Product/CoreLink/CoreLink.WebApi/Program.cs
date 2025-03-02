using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var sqlConnectionString = builder.Configuration["SqlConnectionString"] ?? "";
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

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
    var connectionStringStatus = sqlConnectionStringFound ? "found" : "not found";
    var passwordOptions = identityOptions.Value.Password;
    var passwordRequirements = $"Required Length: {passwordOptions.RequiredLength}, Require Digit: {passwordOptions.RequireDigit}, Require Lowercase: {passwordOptions.RequireLowercase}, Require Uppercase: {passwordOptions.RequireUppercase}, Require Non-Alphanumeric: {passwordOptions.RequireNonAlphanumeric}";

    var buildDate = System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location);
    var deployDate = DateTime.UtcNow; // Assuming the current time as deploy time

    return $"The API is up and running. Environment: {environment}. Connection string: {connectionStringStatus}. Password Requirements: {passwordRequirements}. Build Date: {buildDate}. Deploy Date: {deployDate}.";
});

app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

app.Run();
