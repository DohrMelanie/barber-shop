using AppServices;

namespace WebApi;

public static class DemoEndpoints
{
    public static IEndpointRouteBuilder MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        // Demonstrates an endpoint returning a string
        app.MapGet("/ping", () => "pong")
            .WithDescription("A simple ping endpoint to check if the service is running.");

        // Demonstrates an endpoint interacting with the database
        // and returning a list of objects.
        app.MapGet("/appointments", (ApplicationDataContext db) => db.Appointments)
            .WithDescription("Gets all appointments from the database.");

        app.MapPost("/generate", GenerateRecords)
        .Produces<List<DemoOutputDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithDescription("Generates a list of DemoOutputDto objects based on the specified number of records.");

        return app;
    }

    public static IResult GenerateRecords(DemoInputDto input)
    {
        if (input.NumberOfRecords < 1 || input.NumberOfRecords > 1000)
        {
            return Results.BadRequest("NumberOfRecords must be between 1 and 1000.");
        }

        var output = Enumerable.Range(1, input.NumberOfRecords)
            .Select(i => new DemoOutputDto(i, $"Name {i}"))
            .ToList();
        return Results.Ok(output);
    }
}

public record DemoInputDto(int NumberOfRecords);

public record DemoOutputDto(int Id, string Name);
