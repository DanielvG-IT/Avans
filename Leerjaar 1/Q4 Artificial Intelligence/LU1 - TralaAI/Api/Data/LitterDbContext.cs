using Microsoft.EntityFrameworkCore;
using Api.Models.Data;
using Api.Models;

namespace Api.Data;

public class LitterDbContext(DbContextOptions<LitterDbContext> options) : DbContext(options)
{
  public DbSet<Litter> Litters { get; set; }
  public DbSet<Camera> Cameras { get; set; }
  public DbSet<ApiKey> ApiKeys { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Litter>()
      .HasOne<Camera>()
      .WithMany()
      .HasForeignKey(l => l.CameraId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}