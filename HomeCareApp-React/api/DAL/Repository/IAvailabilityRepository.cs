using HomeCareApp.Models;

namespace HomeCareApp.DAL;

public interface IAvailabilityRepository
{
    Task<List<Availability>> GetAllAsync();
    Task<Availability?> GetByIdAsync(int id);
    Task AddAsync(Availability availability);
    Task UpdateAsync(Availability availability);
    Task DeleteAsync(int id);
}
