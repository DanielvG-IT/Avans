using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TralaAI.CoreApi.Interfaces;
using TralaAI.CoreApi.Data;

namespace TralaAI.CoreApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class MonitoringController : ControllerBase
    {
        private readonly LitterDbContext _context;

        public MonitoringController(LitterDbContext context)
        {
            _context = context;
        }
    }

}