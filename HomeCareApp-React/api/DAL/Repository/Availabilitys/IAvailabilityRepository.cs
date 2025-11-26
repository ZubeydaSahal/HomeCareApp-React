using HomeCareApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCareApp.DAL.Repository.Availabilitys
{
    public interface IAvailabilityRepository
    {
        Task<List<Availability>> GetAllAsync();
        Task<Availability?> GetByIdAsync(int id);
        
        Task AddAsync(Availability availability);
        Task UpdateAsync(Availability availability);
        Task DeleteAsync(int id);
    }
}
