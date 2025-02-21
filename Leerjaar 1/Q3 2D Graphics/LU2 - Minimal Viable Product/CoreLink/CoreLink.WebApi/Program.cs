using CoreLink.WebApi.Interfaces;
using CoreLink.WebApi.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
var sqlConnectionString = builder.Configuration["SqlConnectionString"]
    ?? throw new InvalidProgramException("Configuration variable SqlConnectionString not found");

// Configure Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 12; // 512 was extreem lang
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

app.UseAuthorization();
app.MapGroup("/account").MapIdentityApi<IdentityUser>();
app.MapControllers().RequireAuthorization();

app.Run();
