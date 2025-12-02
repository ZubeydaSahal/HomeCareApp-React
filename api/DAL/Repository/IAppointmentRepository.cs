using HomeCareApp.Models;
namespace HomeCareApp.DAL;
public interface IAppointmentRepository
{
    //Repository interface for managing appointments
    // //also provids CRUD operations
    Task<List<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(int id);

    Task<List<Appointment>> GetByClientIdAsync(string clientId);
    Task CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

