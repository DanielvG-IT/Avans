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
    public class ArtsController : ControllerBase
    {
        private readonly ZorgAppDbContext _context;

        public ArtsController(ZorgAppDbContext context)
        {
            _context = context;
        }

        // GET: api/Arts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Arts>>> GetArtsen()
        {
            return await _context.Artsen.ToListAsync();
        }

        // GET: api/Arts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Arts>> GetArts(Guid id)
        {
            var arts = await _context.Artsen.FindAsync(id);

            if (arts == null)
            {
                return NotFound();
            }

            return arts;
        }

        // PUT: api/Arts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArts(Guid id, Arts arts)
        {
            if (id != arts.ID)
            {
                return BadRequest();
            }

            _context.Entry(arts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtsExists(id))
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

        // POST: api/Arts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Arts>> PostArts(Arts arts)
        {
            _context.Artsen.Add(arts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArts", new { id = arts.ID }, arts);
        }

        // DELETE: api/Arts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArts(Guid id)
        {
            var arts = await _context.Artsen.FindAsync(id);
            if (arts == null)
            {
                return NotFound();
            }

            _context.Artsen.Remove(arts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtsExists(Guid id)
        {
            return _context.Artsen.Any(e => e.ID == id);
        }
    }
}
