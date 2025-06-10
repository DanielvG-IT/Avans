using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
  public class LitterDbContext(DbContextOptions<LitterDbContext> options) : DbContext(options)
  {
    public DbSet<Litter> Litters { get; set; }
  }
}