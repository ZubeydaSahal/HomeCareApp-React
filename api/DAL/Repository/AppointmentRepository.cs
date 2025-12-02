using HomeCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareApp.DAL
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HomeCareDbContext _context;
        private readonly ILogger<AppointmentRepository> _logger; // logger

        public AppointmentRepository(HomeCareDbContext context, ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger; // injected logger
        }

        //Gets all appointments
        public async Task<List<Appointment>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all appointments"); // info

                var list = await _context.Appointments
                    .Include(a => a.Availability)
                    .ThenInclude(av => av.Personnel)
                    .Include(a => a.Client)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} appointments", list.Count); // info

                return list
                    .OrderByDescending(a => a.Availability!.Date)
                    .ThenByDescending(a => a.StartTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all appointments"); // error
                throw; // rethrow so controllers can handle it
            }
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching appointment {Id}", id); // info

                var appt = await _context.Appointments
                    .Include(a => a.Availability)
                    .ThenInclude(av => av.Personnel)
                    .Include(a => a.Client)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appt == null)
                    _logger.LogWarning("Appointment {Id} not found", id); // warning

                return appt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment {Id}", id); // error
                throw;
            }
        }

        //gets appointments by the client Id
        public async Task<List<Appointment>> GetByClientIdAsync(string clientId)
        {
            try
            {
                _logger.LogInformation("Fetching appointments for client {ClientId}", clientId); // info

                var list = await _context.Appointments
                    .Where(a => a.ClientId == clientId)
                    .Include(a => a.Availability)
                    .ThenInclude(av => av.Personnel)
                    .Include(a => a.Client)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} appointments for client {ClientId}",
                    list.Count, clientId); // info

                return list
                    .OrderByDescending(a => a.Availability!.Date)
                    .ThenByDescending(a => a.StartTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments for client {ClientId}", clientId); // error
                throw;
            }
        }
        //creates new appointment 
        public async Task CreateAsync(Appointment appointment)
        {
            try
            {
                _logger.LogInformation("Creating appointment for client {ClientId}", appointment.ClientId); // info

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created appointment {Id}", appointment.Id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment for client {ClientId}", appointment.ClientId); // error
                throw;
            }
        }

        //updates an appointment
        public async Task UpdateAsync(Appointment appointment)
        {
            try
            {
                _logger.LogInformation("Updating appointment {Id}", appointment.Id); // info

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated appointment {Id}", appointment.Id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {Id}", appointment.Id); // error
                throw;
            }
        }
        //deletes appointment by the id
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting appointment {Id}", id); // info

                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment {Id} not found for deletion", id); // warning
                    return;
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted appointment {Id}", id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {Id}", id); // error
                throw;
            }
        }

        public Task<bool> ExistsAsync(int id)
        {
            try
            {
                _logger.LogInformation("Checking existence of appointment {Id}", id); // info
                return _context.Appointments.AnyAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence for appointment {Id}", id); // error
                throw;
            }
        }
    }
}
