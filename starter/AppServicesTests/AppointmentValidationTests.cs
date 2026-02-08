using AppServices;
using Xunit;

namespace AppServicesTests;

/// <summary>
/// Unit tests for appointment validation logic.
/// Students must add at least 5 tests covering all validation rules.
/// </summary>
public class AppointmentValidationTests
{
    // TODO for students: Create tests for validation logic
    
    // Example test structure:
    // [Fact]
    // public async Task ValidateAppointment_OnMonday_Returns400Error()
    // {
    //     // Arrange
    //     var appointment = new Appointment 
    //     { 
    //         Date = new DateOnly(2024, 3, 4), // Monday
    //         ...
    //     };
    //     
    //     // Act
    //     var result = await validator.Validate(appointment);
    //     
    //     // Assert
    //     Assert.False(result.IsValid);
    //     Assert.Equal(400, result.StatusCode);
    //     Assert.Contains("closed Monday-Thursday", result.ErrorMessage);
    // }
    
    // TODO: Test weekday restriction (Mon-Thu should be rejected with 400)
    // TODO: Test service conflict (CleanShaven + BeardShaped should be rejected)
    // TODO: Test duration validation (insufficient duration should be rejected)
    // TODO: Test barber availability (Gerrit outside peak hours should be rejected)
    // TODO: Test time conflict detection (overlapping appointment should return 409)
}
