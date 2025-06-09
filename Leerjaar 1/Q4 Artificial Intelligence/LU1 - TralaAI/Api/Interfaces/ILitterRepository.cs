using Api.Models;

namespace Api.Interfaces
{
    public interface ILitterRepository
    {
        Task<List<Litter>> GetAllAsync();
        Task<Litter?> GetByIdAsync(int id);
        Task AddAsync(Litter litter);
        void Remove(Litter litter);
        Task SaveChangesAsync();
    }
}