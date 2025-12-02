using HomeCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareApp.DAL
{

    //Repository for managing availability
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly HomeCareDbContext _context;
        private readonly ILogger<AvailabilityRepository> _logger; // logger

        public AvailabilityRepository(HomeCareDbContext context, ILogger<AvailabilityRepository> logger)
        {
            _context = context;
            _logger = logger; // injected logger
        }
        
        //Gets all availability 
        public async Task<List<Availability>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all availability entries"); // info

                var list = await _context.Availabilities
                    .Include(a => a.Personnel)
                    .Include(a => a.Appointment)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} availability entries", list.Count); // info

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all availability entries"); // error
                throw;
            }
        }
        //Get a single availability by the id
        public async Task<Availability?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching availability {Id}", id); // info

                var result = await _context.Availabilities
                    .Include(a => a.Personnel)
                    .Include(a => a.Appointment)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (result == null)
                    _logger.LogWarning("Availability {Id} not found", id); // warning

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availability {Id}", id); // error
                throw;
            }
        }

        // Adds a new availability
        public async Task AddAsync(Availability availability)
        {
            try
            {
                _logger.LogInformation("Adding availability for personnel {PersonnelId}", availability.PersonnelId); // info

                _context.Availabilities.Add(availability);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Availability {Id} created", availability.Id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding availability for personnel {PersonnelId}", availability.PersonnelId); // error
                throw;
            }
        }

        //Update availability
        public async Task UpdateAsync(Availability availability)
        {
            try
            {
                _logger.LogInformation("Updating availability {Id}", availability.Id); // info

                _context.Availabilities.Update(availability);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Availability {Id} updated", availability.Id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability {Id}", availability.Id); // error
                throw;
            }
        }
        
        // Delete availability by id
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting availability {Id}", id); // info

                var availability = await _context.Availabilities.FindAsync(id);
                if (availability == null)
                {
                    _logger.LogWarning("Availability {Id} not found for deletion", id); // warning
                    return;
                }

                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Availability {Id} deleted", id); // info
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting availability {Id}", id); // error
                throw;
            }
        }
    }
}
