using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HomeCareApp.Models;

namespace HomeCareApp.DAL
{
    public class HomeCareDbContext : IdentityDbContext<User>
    {
        //entity Framework Core db context for HomeCareApp
        public HomeCareDbContext(DbContextOptions<HomeCareDbContext> options) : base(options)
        {
            //Database.EnsureCreated();

        }

        public DbSet<Availability> Availabilities => Set<Availability>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //configure one to one between availability and appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Availability)
                .WithOne(av => av.Appointment)
                .HasForeignKey<Appointment>(a => a.AvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
