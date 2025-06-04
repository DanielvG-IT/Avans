using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
  public class LitterDbContext(DbContextOptions<LitterDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {

  }
}