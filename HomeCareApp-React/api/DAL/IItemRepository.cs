using HomeCareApp.Models;

namespace HomeCareApp.DAL
{
    public interface IItemRepository
    {
        Task<IEnumerable<Availability>?> GetAllAvailabilitiesAsync();
        Task<Availability?> GetAvailabilityByIdAsync(int id);
        Task <bool> AddAvailabilityAsync(Availability availability);
        Task <bool> UpdateAvailabilityAsync(Availability availability);
        Task <bool> DeleteAvailabilityAsync(int id);
    }
   
}