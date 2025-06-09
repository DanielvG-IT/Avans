using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TralaAI.CoreApi.Data;
using TralaAI.CoreApi.Services;
using TralaAI.CoreApi.Interfaces;
using TralaAI.CoreApi.Repositories;
using TralaAI.CoreApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🛠️ Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<LitterDbContext>(
    options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
/* TODO: Add options for passwords ect...*/
);
builder.Services.AddScoped<ILitterRepository, LitterRepository>();
builder.Services.AddScoped<IAggregatedTrashService, AggregatedTrashService>();
builder.Services.AddHttpClient<IAggregatedTrashService, AggregatedTrashService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
