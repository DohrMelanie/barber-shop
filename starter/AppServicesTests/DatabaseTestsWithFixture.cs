using AppServices;
using Microsoft.EntityFrameworkCore;
using TestInfrastructure;

namespace AppServicesTests;

public class DatabaseTestsWithClassFixture(DatabaseFixture fixture)
    : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CanAddAndRetrieveAppointment()
    {
        // Arrange
        int appointmentId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var appt = new Appointment
            {
                CustomerName = "Test Customer",
                Date = new DateOnly(2023, 10, 1),
                StartTime = new TimeOnly(14, 0),
                Duration = TimeSpan.FromMinutes(30),
                BarberName = "Sweeney Todd"
            };
            context.Appointments.Add(appt);
            await context.SaveChangesAsync();
            appointmentId = appt.Id;
        }

        // Act & Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var match = await context.Appointments.FindAsync(appointmentId);
            Assert.NotNull(match);
            Assert.Equal("Test Customer", match.CustomerName);
            Assert.Equal("Sweeney Todd", match.BarberName);
            Assert.Equal(new DateOnly(2023, 10, 1), match.Date);
        }
    }

    [Fact]
    public async Task CanAddAppointmentWithServices()
    {
        // Arrange
        int appointmentId;
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var appt = new Appointment
            {
                CustomerName = "Fancy Customer",
                Date = new DateOnly(2024, 1, 1),
                StartTime = new TimeOnly(10, 0),
                Duration = TimeSpan.FromHours(1)
            };
            
            appt.Services.Add(new AppointmentService { Name = "Haircut", Price = 30m });
            appt.Services.Add(new AppointmentService { Name = "Shave", Price = 20m });

            context.Appointments.Add(appt);
            await context.SaveChangesAsync();
            appointmentId = appt.Id;
        }

        // Act & Assert
        await using (var context = new ApplicationDataContext(fixture.Options))
        {
            var match = await context.Appointments
                .Include(a => a.Services)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            Assert.NotNull(match);
            Assert.Equal(2, match.Services.Count);
            Assert.Contains(match.Services, s => s.Name == "Haircut");
        }
    }
}
