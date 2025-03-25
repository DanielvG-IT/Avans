using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
public interface IGuardianRepository
{
    Task<IEnumerable<Guardian>> GetAllGuardiansAsync();
    Task<Guardian?> GetGuardianByIdAsync(Guid id);
    Task<int> AddGuardianAsync(Guardian guardian);
    Task<int> UpdateGuardianAsync(Guardian guardian);
    Task<int> DeleteGuardianAsync(Guid id);
}