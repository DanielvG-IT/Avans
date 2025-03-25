namespace HQB.WebApi.Interfaces;

public interface ITreatmentRepository
{
    Task<IEnumerable<Treatment>> GetAllTreatmentsAsync();
    Task<Treatment?> GetTreatmentByIdAsync(Guid id);
    Task<int> AddTreatmentAsync(Treatment treatment);
    Task<int> UpdateTreatmentAsync(Treatment treatment);
    Task<int> DeleteTreatmentAsync(Guid id);
}