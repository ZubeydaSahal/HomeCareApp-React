using HomeCareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCareApp.DAL;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly HomeCareDbContext _context;
    public AppointmentRepository(HomeCareDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAllAsync()
    {
        var list = await _context.Appointments
           .Include(a => a.Availability)
           .ThenInclude(av => av.Personnel)
           .Include(a => a.Client)
            .ToListAsync();

        return list
            .OrderByDescending(a => a.Availability!.Date)
            .ThenByDescending(a => a.StartTime)
            .ToList();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Availability)
            .ThenInclude(av => av.Personnel)
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Appointment>> GetByClientIdAsync(string clientId)
    {
        var list = await _context.Appointments
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Availability)
            .ThenInclude(av => av.Personnel)
            .Include(a => a.Client)
            .ToListAsync();

        return list
            .OrderByDescending(a => a.Availability!.Date)
            .ThenByDescending(a => a.StartTime)
            .ToList();
    }

    public async Task CreateAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Appointments.AnyAsync(a => a.Id == id);
    }
}

