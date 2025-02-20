using CoreLink.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreLink.WebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
      return Ok();
    }


  }
}