using HQB.WebApi.Interfaces;
using HQB.WebApi.Repositories;
using Microsoft.AspNetCore.Identity;

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
app.Run();