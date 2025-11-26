using HomeCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareApp.DAL
{
    public class ItemRepository : IItemRepository
    {
        private readonly HomeCareDbContext _context;
        private readonly ILogger<ItemRepository>? _logger;

        public ItemRepository(HomeCareDbContext context, ILogger<ItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ItemRepository(HomeCareDbContext context)
        {
            _context = context;
            _logger = null;
        }

        public async Task<IEnumerable<Availability>?> GetAllAvailabilitiesAsync()
        {
            try
            {
                return await _context.Availabilities.ToListAsync();
            }
            catch (Exception e)
            {
                _logger?.LogError("[ItemRepository] items ToListAsync() failed when GetAll(), error message: {e}", e.Message);
                return null;
            }
        }

        public async Task<Availability?> GetAvailabilityByIdAsync(int id)
        {
            try
            {
                return await _context.Availabilities.FindAsync(id);
            }
            catch (Exception e)
            {
                _logger?.LogError("[Itemrepository] item FindAsync(id) failed when GetItemById for ItemId {ItemId:0000}, error message: {e}", id, e.Message);
                return null;

            }
        }

        public async Task<bool> AddAvailabilityAsync(Availability availability)
        {
            try
            {
                _context.Availabilities.Add(availability);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError("[ItemRepository] item creation failed for item {@item}, error message: {e}", availability, e.Message);
                return false;

            }
            
        }

        public async Task<bool> UpdateAvailabilityAsync(Availability availability)
        {
            try
            {
                _context.Availabilities.Update(availability);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError("[ItemRepository] item FindAsync(id) failed when updating the ItemId {ItemId:0000}, error message: {e}", availability, e.Message);
                return false;
            }
            
        }

        public async Task<bool> DeleteAvailabilityAsync(int id)
        {
            try
            {
                var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null)
            {
                _logger?.LogError("[ItemRepository] item not found for the ItemId {ItemId:0000}", id);
                return false;
            }

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return true;
            }
            catch (Exception e)
            {
                _logger?.LogError("[ItemRepository] item deletion failed for the ItemId {ItemId:0000}, error message: {e}", id, e.Message);
                return false;

            }
        }
    }
}