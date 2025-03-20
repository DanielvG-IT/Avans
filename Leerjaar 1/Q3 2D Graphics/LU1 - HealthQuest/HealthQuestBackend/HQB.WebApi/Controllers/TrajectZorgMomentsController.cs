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
    public class TrajectZorgMomentsController(ZorgAppDbContext context) : ControllerBase
    {
        private readonly ZorgAppDbContext _context = context;

        // GET: api/TrajectZorgMoments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrajectZorgMoment>>> GetTrajectZorgMomenten()
        {
            return await _context.TrajectZorgMomenten.ToListAsync();
        }

        // GET: api/TrajectZorgMoments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrajectZorgMoment>> GetTrajectZorgMoment(int id)
        {
            var trajectZorgMoment = await _context.TrajectZorgMomenten.FindAsync(id);

            if (trajectZorgMoment == null)
            {
                return NotFound();
            }

            return trajectZorgMoment;
        }

        // PUT: api/TrajectZorgMoments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrajectZorgMoment(int id, TrajectZorgMoment trajectZorgMoment)
        {
            if (id != trajectZorgMoment.TrajectID)
            {
                return BadRequest();
            }

            _context.Entry(trajectZorgMoment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrajectZorgMomentExists(id))
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

        // POST: api/TrajectZorgMoments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TrajectZorgMoment>> PostTrajectZorgMoment(TrajectZorgMoment trajectZorgMoment)
        {
            _context.TrajectZorgMomenten.Add(trajectZorgMoment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TrajectZorgMomentExists(trajectZorgMoment.TrajectID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTrajectZorgMoment", new { id = trajectZorgMoment.TrajectID }, trajectZorgMoment);
        }

        // DELETE: api/TrajectZorgMoments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrajectZorgMoment(int id)
        {
            var trajectZorgMoment = await _context.TrajectZorgMomenten.FindAsync(id);
            if (trajectZorgMoment == null)
            {
                return NotFound();
            }

            _context.TrajectZorgMomenten.Remove(trajectZorgMoment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrajectZorgMomentExists(int id)
        {
            return _context.TrajectZorgMomenten.Any(e => e.TrajectID == id);
        }
    }
}
