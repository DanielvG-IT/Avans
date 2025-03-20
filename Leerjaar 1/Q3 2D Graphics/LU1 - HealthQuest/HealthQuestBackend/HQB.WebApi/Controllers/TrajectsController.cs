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
    public class TrajectsController(ZorgAppDbContext context) : ControllerBase
    {
        private readonly ZorgAppDbContext _context = context;

        // GET: api/Trajects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Traject>>> GetTrajecten()
        {
            return await _context.Trajecten.ToListAsync();
        }

        // GET: api/Trajects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Traject>> GetTraject(int id)
        {
            var traject = await _context.Trajecten.FindAsync(id);

            if (traject == null)
            {
                return NotFound();
            }

            return traject;
        }

        // PUT: api/Trajects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTraject(int id, Traject traject)
        {
            if (id != traject.ID)
            {
                return BadRequest();
            }

            _context.Entry(traject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrajectExists(id))
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

        // POST: api/Trajects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Traject>> PostTraject(Traject traject)
        {
            _context.Trajecten.Add(traject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTraject", new { id = traject.ID }, traject);
        }

        // DELETE: api/Trajects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTraject(int id)
        {
            var traject = await _context.Trajecten.FindAsync(id);
            if (traject == null)
            {
                return NotFound();
            }

            _context.Trajecten.Remove(traject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrajectExists(int id)
        {
            return _context.Trajecten.Any(e => e.ID == id);
        }
    }
}
