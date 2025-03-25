using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianRepository _guardianRepository;

        public GuardianController(IGuardianRepository ouderVoogdRepository)
        {
            _guardianRepository = ouderVoogdRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetOuderVoogd()
        {
            var guardians = await _guardianRepository.GetAllGuardiansAsync();
            return Ok(guardians);
        }

        [HttpPost]
        public async Task<IActionResult> PostOuderVoogd([FromBody] Guardian guardian)
        {
            var result = await _guardianRepository.AddGuardianAsync(guardian);
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetOuderVoogdById), new { id = guardian.ID }, guardian);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOuderVoogdById(Guid id)
        {
            var guardian = await _guardianRepository.GetGuardianByIdAsync(id);
            if (guardian == null)
            {
                return NotFound();
            }
            return Ok(guardian);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutOuderVoogd(Guid id, [FromBody] Guardian guardian)
        {
            if (id != guardian.ID)
            {
                return BadRequest();
            }

            var result = await _guardianRepository.UpdateGuardianAsync(guardian);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOuderVoogd(Guid id)
        {
            var result = await _guardianRepository.DeleteGuardianAsync(id);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
