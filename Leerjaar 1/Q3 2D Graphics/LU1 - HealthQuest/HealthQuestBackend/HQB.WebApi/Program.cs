using System.Text;
using HQB.WebApi.Data;
using HQB.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Load Configuration
var configuration = builder.Configuration;

// 🔹 Add Database Context (EF Core with SQL Server)
builder.Services.AddDbContext<ZorgAppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("HealthQuestConnectionString")));

// 🔹 Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
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

// 🔹 Configure Middleware
if (app.Environment.IsDevelopment())
{
    // 🔹 Add API Documentation
    app.MapOpenApi();
    builder.Services.AddEndpointsApiExplorer();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();