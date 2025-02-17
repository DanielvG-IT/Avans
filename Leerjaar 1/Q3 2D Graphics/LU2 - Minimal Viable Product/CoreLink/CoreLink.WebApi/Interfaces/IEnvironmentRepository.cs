using CoreLink.WebApi.Models;

namespace CoreLink.WebApi.Interfaces
{
    public interface IEnvironmentRepository
    {
        Task CreateAsync(Environment2D environment);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Environment2D>> ReadAsync();
        Task<Environment2D?> ReadAsync(Guid Id);
        Task UpdateAsync(Environment2D environment);
    }
}
