using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(int id);
    Task<int> AddPatientAsync(Patient patient);
    Task<int> UpdatePatientAsync(Patient patient);
    Task<int> DeletePatientAsync(int id);
}