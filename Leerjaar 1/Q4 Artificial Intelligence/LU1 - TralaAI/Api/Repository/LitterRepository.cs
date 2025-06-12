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

        public async Task AddAsync(Litter litter)
        {
            await _context.Litters.AddAsync(litter);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Litter>> GetFilteredAsync(LitterFilterDto filter)
        {
            var query = _context.Litters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Type))
                query = query.Where(x => x.Type == filter.Type);

            if (filter.From.HasValue)
                query = query.Where(x => x.Date >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(x => x.Date <= filter.To.Value);

            if (filter.MinTemperature.HasValue)
                query = query.Where(x => x.Temperature >= filter.MinTemperature.Value);

            if (filter.MaxTemperature.HasValue)
                query = query.Where(x => x.Temperature <= filter.MaxTemperature.Value);


            return await query.ToListAsync();
        }
    }
}