using HomeCareApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HomeCareApp.DAL
{
    public static class DBInit
    {

        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            var db = services.GetRequiredService<HomeCareDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Run async seed synchronously at startup
            SeedAsync(db, userManager, roleManager).GetAwaiter().GetResult();
        }

        public static async Task SeedAsync(
            HomeCareDbContext db,
            UserManager<User> um,
            RoleManager<IdentityRole> rm)
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            // Create roles
            var roles = new[] { "Admin", "Personnel", "Patient" };
            foreach (var role in roles)
            {
                if (!await rm.RoleExistsAsync(role))
                    await rm.CreateAsync(new IdentityRole(role));
            }

            // create test users
            var nurseEmail = "nurse@homecare.local";
            var patientEmail = "patient@homecare.local";
            var adminEmail = "admin@homecare.local";

            var admin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Admin Alice",
                Role = "Admin"
            };

            var adminResult = await um.CreateAsync(admin, "Pass123!");
            if (adminResult.Succeeded)
            {
                await um.AddToRoleAsync(admin, "Admin");
                await um.AddToRoleAsync(admin, "Personnel");
            }

            var nurse = new User
            {
                UserName = nurseEmail,
                Email = nurseEmail,
                EmailConfirmed = true,
                FullName = "Nurse Nora",
                Role = "Personnel"
            };
            var nurseResult = await um.CreateAsync(nurse, "Pass123!");
            if (nurseResult.Succeeded)
                await um.AddToRoleAsync(nurse, "Personnel");

            var patient = new User
            {
                UserName = patientEmail,
                Email = patientEmail,
                EmailConfirmed = true,
                FullName = "Patient Peter",
                Role = "Patient"
            };
            var patientResult = await um.CreateAsync(patient, "Pass123!");
            if (patientResult.Succeeded)
                await um.AddToRoleAsync(patient, "Patient");

            // create Availability
            var today = DateTime.Today;
            var availabilities = new List<Availability>
            {
                new Availability
                {
                    PersonnelId = nurse.Id,
                    Date = today,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    Notes = "Morning visit"
                },
                new Availability
                {
                    PersonnelId = nurse.Id,
                    Date = today,
                    StartTime = new TimeSpan(11, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Notes = "Check-up"
                },
                new Availability
                {
                    PersonnelId = nurse.Id,
                    Date = today.AddDays(1),
                    StartTime = new TimeSpan(9, 30, 0),
                    EndTime = new TimeSpan(10, 30, 0),
                    Notes = "Follow-up"
                },
                new Availability
                {
                    PersonnelId = nurse.Id,
                    Date = today.AddDays(2),
                    StartTime = new TimeSpan(13, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    Notes = "Home visit"
                }
            };

            db.Availabilities.AddRange(availabilities);
            await db.SaveChangesAsync();

            //  create a couple of appointments
            var bookedAppointments = new List<Appointment>
            {
                new Appointment
                {
                    ClientId = patient.Id,
                    AvailabilityId = availabilities[0].Id,
                    TaskDescription = "Medication reminder",
                    StartTime = availabilities[0].StartTime,
                    EndTime = availabilities[0].EndTime,
                    Status = "Booked"
                },
                new Appointment
                {
                    ClientId = patient.Id,
                    AvailabilityId = availabilities[2].Id,
                    TaskDescription = "Routine check",
                    StartTime = availabilities[2].StartTime,
                    EndTime = availabilities[2].EndTime,
                    Status = "Booked"
                }
            };

            db.Appointments.AddRange(bookedAppointments);
            await db.SaveChangesAsync();
        }
    }
}
