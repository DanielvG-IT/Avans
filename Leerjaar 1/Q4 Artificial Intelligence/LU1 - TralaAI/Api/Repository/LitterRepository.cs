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
                query = query.Where(x => x.LitterCategory == filter.Type);

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

        public async Task<List<Litter>> GetLatestAsync(int amoutOfRecords = 100)
        {
            return await _context.Litters.OrderByDescending(l => l.TimeStamp).Take(amoutOfRecords).ToListAsync();
        }

        public async Task<List<LitterAmountCamera>> GetAmountPerCameraAsync()
        {
            var query = _context.Cameras
                .GroupJoin(
                    _context.Litters,
                    camera => camera.Id,
                    litter => litter.CameraId,
                    (camera, litters) => new LitterAmountCamera
                    {
                        CameraId = camera.Id,
                        Organic = litters.Count(l => l.LitterCategory == LitterCategory.Organic),
                        Plastic = litters.Count(l => l.LitterCategory == LitterCategory.Plastic),
                        Paper = litters.Count(l => l.LitterCategory == LitterCategory.Paper),
                        Glass = litters.Count(l => l.LitterCategory == LitterCategory.Glass),
                        Metal = litters.Count(l => l.LitterCategory == LitterCategory.Metal)
                    }
                );

            return await query.ToListAsync();
        }
    }
}