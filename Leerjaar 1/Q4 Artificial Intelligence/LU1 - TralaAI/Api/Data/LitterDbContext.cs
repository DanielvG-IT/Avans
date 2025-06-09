using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
  public class LitterDbContext(DbContextOptions<LitterDbContext> options) : IdentityDbContext(options)
  {
    public DbSet<Litter> Litters { get; set; }
  }
}