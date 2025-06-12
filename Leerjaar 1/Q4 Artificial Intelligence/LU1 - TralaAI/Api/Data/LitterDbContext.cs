using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
  public class LitterDbContext(DbContextOptions<LitterDbContext> options) : DbContext(options)
  {
    public DbSet<Litter> Litters { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Seed a default API key
      modelBuilder.Entity<ApiKey>().HasData(
        new ApiKey
        {
          Id = 1,
          Key = Guid.Parse("b7e2c8c7-3f9a-4e2b-8e2d-1a2b3c4d5e6f"),
          CreatedAt = DateTime.UtcNow,
          ExpiresAt = DateTime.UtcNow.AddYears(1),
          IsActive = true
        }
      );
    }
  }
}