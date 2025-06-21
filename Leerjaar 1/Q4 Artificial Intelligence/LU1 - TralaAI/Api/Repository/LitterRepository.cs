using Microsoft.EntityFrameworkCore;
using Api.Models.Enums;
using Api.Models.Data;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Repository
{
    public class LitterRepository(LitterDbContext context) : ILitterRepository
    {
        private readonly LitterDbContext _context = context;

        public async Task AddAsync(Litter litter)
        {
            await _context.Litters.AddAsync(litter);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Camera>?> GetCamerasAsync()
        {
            return await _context.Cameras.ToListAsync();
        }

        public async Task<List<Litter>> GetFilteredAsync(LitterFilterDto filter)
        {
            var query = _context.Litters.AsQueryable();

            if (filter.Type.HasValue)
                query = query.Where(x => x.Type == filter.Type.Value);

            if (filter.From.HasValue)
                query = query.Where(x => x.TimeStamp >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(x => x.TimeStamp <= filter.To.Value);

            if (filter.MinTemperature.HasValue)
                query = query.Where(x => x.Temperature >= filter.MinTemperature.Value);

            if (filter.MaxTemperature.HasValue)
                query = query.Where(x => x.Temperature <= filter.MaxTemperature.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Litter>> GetLatestAsync(int? amoutOfRecords = null)
        {
            var query = _context.Litters.AsQueryable();

            if (amoutOfRecords is not null && amoutOfRecords > 0)
            {
                query = query.OrderByDescending(l => l.TimeStamp).Take(amoutOfRecords.Value);
            }
            else
            {
                query = query.OrderByDescending(l => l.TimeStamp).Take(100);
            }

            return await query.ToListAsync();
        }

        public async Task<LitterTypeAmount?> GetAmountPerLocationAsync()
        {
            var query = _context.Litters.AsQueryable();

            var groupedQuery = await query.GroupBy(l => l.CameraId)
                            .Select(g => new LitterTypeAmount
                            {
                                Organic = g.Count(l => l.Type == LitterCategory.Organic),
                                Paper = g.Count(l => l.Type == LitterCategory.Paper),
                                Plastic = g.Count(l => l.Type == LitterCategory.Plastic),
                                Glass = g.Count(l => l.Type == LitterCategory.Glass),
                                Metal = g.Count(l => l.Type == LitterCategory.Metal)
                            })
                            .OrderByDescending(l => l.Organic + l.Paper + l.Plastic + l.Glass + l.Metal)
                            .FirstOrDefaultAsync();

            return groupedQuery;
        }
    }
}