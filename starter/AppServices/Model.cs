namespace AppServices;

public class Appointment
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeSpan Duration { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public List<AppointmentService> Services { get; set; } = [];

    public string? BarberName { get; set; }
    public string? BeverageChoice { get; set; }
    public bool IsVip { get; set; }
}

public class AppointmentService
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public StyleReference StyleReference { get; set; }
    public int AppointmentId { get; set; }
}

public enum StyleReference
{
    // Base length / structure
    Short,
    Medium,
    Long,

    // Cut characteristics
    Faded,
    Tapered,
    Undercut,
    Layered,
    Textured,

    // Styling
    SlickedBack,
    SideParted,
    ForwardCrop,
    Voluminous,
    Natural,

    // Statement styles
    MulletStyle,
    MohawkStyle,

    // Beard / services
    BeardShaped,
    CleanShaven,
    HotTowelShave
}
