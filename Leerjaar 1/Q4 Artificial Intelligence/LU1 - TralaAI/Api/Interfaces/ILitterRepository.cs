using Api.Models;

namespace Api.Interfaces
{
    public interface ILitterRepository
    {
        Task AddAsync(Litter litter);
        Task SaveChangesAsync();
        Task<List<Litter>> GetFilteredAsync(LitterFilterDto filter);
    }
}