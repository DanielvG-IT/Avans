using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TralaAI.CoreApi.Models;

namespace TralaAI.CoreApi.Data
{
  public class LitterDbContext : IdentityDbContext
  {
    public LitterDbContext(DbContextOptions<LitterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Litter> Litters { get; set; }
  }
}