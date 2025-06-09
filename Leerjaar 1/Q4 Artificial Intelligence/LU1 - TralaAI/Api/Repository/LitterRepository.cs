using Microsoft.EntityFrameworkCore;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Repository
{
    public class LitterRepository : ILitterRepository
    {
        private readonly LitterDbContext _context;

        public LitterRepository(LitterDbContext context)
        {
            _context = context;
        }

        public async Task<List<Litter>> GetAllAsync()
        {
            return await _context.Litters.ToListAsync();
        }

        public async Task<Litter?> GetByIdAsync(int id)
        {
            return await _context.Litters.FindAsync(id);
        }

        public async Task AddAsync(Litter litter)
        {
            await _context.Litters.AddAsync(litter);
        }

        public void Remove(Litter litter)
        {
            _context.Litters.Remove(litter);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}