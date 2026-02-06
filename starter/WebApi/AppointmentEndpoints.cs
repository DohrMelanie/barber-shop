using AppServices;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public static class AppointmentEndpoints
{
    public static IEndpointRouteBuilder MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/appointments")
            .WithTags("Appointments")
            .WithDescription("Manage barber shop appointments.");

        // TODO: Implement GET /appointments to retrieve all appointments
        // TODO: Implement GET /appointments/{id} to retrieve a specific appointment
        // TODO: Implement POST /appointments to create a new appointment
        // TODO: Implement PUT /appointments/{id} to update an existing appointment
        // TODO: Implement DELETE /appointments/{id} to delete an appointment

        return app;
    }
}
