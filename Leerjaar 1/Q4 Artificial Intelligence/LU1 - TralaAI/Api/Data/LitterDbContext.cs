using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TralaAI.CoreApi.Models;

namespace TralaAI.CoreApi.Data
{
  public class LitterDbContext(DbContextOptions<LitterDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {

  }
}