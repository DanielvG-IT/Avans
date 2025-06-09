using Microsoft.AspNetCore.Mvc;
using Api.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class MonitoringController(LitterDbContext context) : ControllerBase
    {
        private readonly LitterDbContext _context = context;
    }
}