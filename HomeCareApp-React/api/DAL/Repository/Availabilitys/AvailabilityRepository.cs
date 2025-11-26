using HomeCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareApp.DAL.Repository.Availabilitys
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly HomeCareDbContext _context;

        public AvailabilityRepository(HomeCareDbContext context)
        {
            _context = context;
        }

        public Task<List<Availability>> GetAllAsync()
        {
            return _context.Availabilities
                .Include(a => a.Personnel)  
                .Include(a => a.Appointment) 
                .ToListAsync();
        }

       public Task<Availability?> GetByIdAsync(int id){
        return _context.Availabilities
        .Include(a => a.Personnel)
        .Include(a => a.Appointment)
        .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Availability availability)
        {
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Availability availability)
        {
            _context.Availabilities.Update(availability);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability != null)
            {
                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
        }
    }
}