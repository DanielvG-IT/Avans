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

        public async Task<List<LitterAmountCamera>> GetAmountPerCameraAsync()
        {
            var cameras = await _context.Cameras.ToListAsync();
            var litters = await _context.Litters.ToListAsync();

            var result = cameras
                .GroupJoin(
                    litters,
                    camera => camera.Id,
                    litter => litter.CameraId,
                    (camera, cameraLitters) => new LitterAmountCamera
                    {
                        CameraId = camera.Id,
                        Organic = cameraLitters.Count(l => l.LitterCategory == LitterCategory.Organic),
                        Plastic = cameraLitters.Count(l => l.LitterCategory == LitterCategory.Plastic),
                        Paper = cameraLitters.Count(l => l.LitterCategory == LitterCategory.Paper),
                        Glass = cameraLitters.Count(l => l.LitterCategory == LitterCategory.Glass),
                        Metal = cameraLitters.Count(l => l.LitterCategory == LitterCategory.Metal)
                    }
                )
                .ToList();

            return result;
        }
    }
}