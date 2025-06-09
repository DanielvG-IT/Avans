using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services;
using Api.Interfaces;
using Api.Repository;

var builder = WebApplication.CreateBuilder(args);

// 🛠️ Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<LitterDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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
