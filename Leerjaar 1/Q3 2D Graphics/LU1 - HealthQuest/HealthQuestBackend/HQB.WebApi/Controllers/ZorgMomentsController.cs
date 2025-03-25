using System;
using System.Linq;
using HQB.WebApi.Data;
using HQB.WebApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HQB.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZorgMomentsController : ControllerBase
    {
        private readonly ZorgAppDbContext _context;

        public ZorgMomentsController(ZorgAppDbContext context)
        {
            _context = context;
        }

        // GET: api/ZorgMoments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ZorgMoment>>> GetZorgMomenten()
        {
            return await _context.ZorgMomenten.ToListAsync();
        }

        // GET: api/ZorgMoments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ZorgMoment>> GetZorgMoment(Guid id)
        {
            var zorgMoment = await _context.ZorgMomenten.FindAsync(id);

            if (zorgMoment == null)
            {
                return NotFound();
            }

            return zorgMoment;
        }

        // PUT: api/ZorgMoments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutZorgMoment(Guid id, ZorgMoment zorgMoment)
        {
            if (id != zorgMoment.ID)
            {
                return BadRequest();
            }

            _context.Entry(zorgMoment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZorgMomentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ZorgMoments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ZorgMoment>> PostZorgMoment(ZorgMoment zorgMoment)
        {
            _context.ZorgMomenten.Add(zorgMoment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetZorgMoment", new { id = zorgMoment.ID }, zorgMoment);
        }

        // DELETE: api/ZorgMoments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZorgMoment(Guid id)
        {
            var zorgMoment = await _context.ZorgMomenten.FindAsync(id);
            if (zorgMoment == null)
            {
                return NotFound();
            }

            _context.ZorgMomenten.Remove(zorgMoment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZorgMomentExists(Guid id)
        {
            return _context.ZorgMomenten.Any(e => e.ID == id);
        }
    }
}
