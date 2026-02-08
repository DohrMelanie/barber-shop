using AppServices;
using AppServices.Importer;
using NSubstitute;

namespace ImporterTests;

public class LegacyFileFixerTests
{
    private readonly IFileReader _mockFileReader;
    private readonly LegacyFileFixer _fixer;

    public LegacyFileFixerTests()
    {
        _mockFileReader = Substitute.For<IFileReader>();
        _fixer = new LegacyFileFixer(_mockFileReader);
    }

    [Fact]
    public async Task ImportAsync_ValidXmlWithSingleAppointment_ReturnsSuccessfulImport()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>John Doe</CustomerName>
        <Date>2024-03-15</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Gerrit</BarberName>
        <Services>HAIRCUT|SHAVE</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        Assert.Empty(result.Failures);
        
        var appointment = result.Successes[0];
        Assert.Equal("John Doe", appointment.CustomerName);
        Assert.Equal(new DateOnly(2024, 3, 15), appointment.Date);
        Assert.Equal(new TimeOnly(10, 0), appointment.StartTime);
        Assert.Equal(TimeSpan.FromMinutes(45), appointment.Duration);
        Assert.Equal("Gerrit", appointment.BarberName);
        Assert.NotEmpty(appointment.Services);
    }

    [Fact]
    public async Task ImportAsync_BrokenXmlWithMultipleRoots_FixesAndImports()
    {
        // Arrange - Broken XML with multiple root elements (no wrapping Root)
        var brokenXml = @"
<Appointment>
    <CustomerName>Alice</CustomerName>
    <Date>2024-03-16</Date>
    <StartTime>14:00</StartTime>
    <Duration>60</Duration>
    <BarberName>Todd</BarberName>
    <Services>CUT</Services>
</Appointment>
<Appointment>
    <CustomerName>Bob</CustomerName>
    <Date>2024-03-17</Date>
    <StartTime>15:00</StartTime>
    <Duration>30</Duration>
    <BarberName>Gerrit</BarberName>
    <Services>SHAVE</Services>
</Appointment>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(brokenXml);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Successes.Count);
        Assert.Empty(result.Failures);
    }

    [Fact]
    public async Task ImportAsync_XmlWithUnescapedAmpersand_FixesAndImports()
    {
        // Arrange - XML with unescaped & character in attribute/content
        var brokenXml = @"
<Appointment>
    <CustomerName>Smith & Sons</CustomerName>
    <Date>2024-03-18</Date>
    <StartTime>11:00</StartTime>
    <Duration>45</Duration>
    <BarberName>Todd</BarberName>
    <Services>HAIRCUT</Services>
</Appointment>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(brokenXml);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        Assert.Equal("Smith & Sons", result.Successes[0].CustomerName);
    }

    [Fact]
    public async Task ImportAsync_DateInGermanFormat_ParsesCorrectly()
    {
        // Arrange - DD.MM.YYYY format
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Hans Mueller</CustomerName>
        <Date>15.03.2024</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        Assert.Equal(new DateOnly(2024, 3, 15), result.Successes[0].Date);
    }

    [Fact]
    public async Task ImportAsync_DateInIsoFormat_ParsesCorrectly()
    {
        // Arrange - YYYY-MM-DD format
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Jane Smith</CustomerName>
        <Date>2024-03-20</Date>
        <StartTime>14:00</StartTime>
        <Duration>60</Duration>
        <BarberName>Gerrit</BarberName>
        <Services>FADE</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        Assert.Equal(new DateOnly(2024, 3, 20), result.Successes[0].Date);
    }

    [Fact]
    public async Task ImportAsync_PipeSeparatedServices_ParsesAllServices()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Mike Johnson</CustomerName>
        <Date>2024-03-21</Date>
        <StartTime>10:00</StartTime>
        <Duration>75</Duration>
        <BarberName>Todd</BarberName>
        <Services>HAIRCUT|SHAVE|BEARD</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        Assert.Equal(3, result.Successes[0].Services.Count);
    }

    [Fact]
    public async Task ImportAsync_MissingCustomerName_ReturnsFailure()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <Date>2024-03-22</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Successes);
        Assert.Single(result.Failures);
        Assert.Equal(ImportError.MissingCompulsoryField, result.Failures[0].Error);
    }

    [Fact]
    public async Task ImportAsync_InvalidDate_ReturnsFailure()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Invalid Date Person</CustomerName>
        <Date>not-a-date</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Successes);
        Assert.Single(result.Failures);
        Assert.Equal(ImportError.InvalidDate, result.Failures[0].Error);
    }

    [Fact]
    public async Task ImportAsync_NoServices_ReturnsFailure()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>No Services Person</CustomerName>
        <Date>2024-03-23</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services></Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Successes);
        Assert.Single(result.Failures);
        Assert.Equal(ImportError.NoServices, result.Failures[0].Error);
    }

    [Fact]
    public async Task ImportAsync_MixedValidAndInvalidRecords_ReturnsPartialSuccess()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Valid Person</CustomerName>
        <Date>2024-03-24</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT</Services>
    </Appointment>
    <Appointment>
        <Date>2024-03-25</Date>
        <StartTime>11:00</StartTime>
        <Duration>30</Duration>
        <BarberName>Gerrit</BarberName>
        <Services>SHAVE</Services>
    </Appointment>
    <Appointment>
        <CustomerName>Another Valid Person</CustomerName>
        <Date>2024-03-26</Date>
        <StartTime>12:00</StartTime>
        <Duration>60</Duration>
        <BarberName>Todd</BarberName>
        <Services>FADE</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Successes.Count);
        Assert.Single(result.Failures);
        Assert.Equal(ImportError.MissingCompulsoryField, result.Failures[0].Error);
    }

    [Fact]
    public async Task ImportAsync_ServiceMapping_MapsToCorrectStyleReferences()
    {
        // Arrange
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Service Test Person</CustomerName>
        <Date>2024-03-27</Date>
        <StartTime>10:00</StartTime>
        <Duration>60</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT|SHAVE|FADE</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        
        var services = result.Successes[0].Services;
        Assert.Equal(3, services.Count);
        
        // Verify services use proper StyleReference enum values
        Assert.Contains(services, s => s.StyleReference == StyleReference.Medium); // CUT
        Assert.Contains(services, s => s.StyleReference == StyleReference.CleanShaven); // SHAVE
        Assert.Contains(services, s => s.StyleReference == StyleReference.Faded); // FADE
    }

    [Fact]
    public async Task ImportAsync_OptionalFields_HandlesNullValues()
    {
        // Arrange - BeverageChoice and IsVip are optional
        var xmlContent = @"
<Root>
    <Appointment>
        <CustomerName>Minimal Person</CustomerName>
        <Date>2024-03-28</Date>
        <StartTime>10:00</StartTime>
        <Duration>45</Duration>
        <BarberName>Todd</BarberName>
        <Services>CUT</Services>
    </Appointment>
</Root>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(xmlContent);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Successes);
        
        var appointment = result.Successes[0];
        Assert.Null(appointment.BeverageChoice);
        Assert.False(appointment.IsVip);
    }

    [Fact]
    public async Task ImportAsync_ComplexBrokenXml_FixesAllIssues()
    {
        // Arrange - Combination of issues: multiple roots, unescaped ampersands, mixed date formats
        var brokenXml = @"
<Appointment>
    <CustomerName>Smith & Jones</CustomerName>
    <Date>15.03.2024</Date>
    <StartTime>10:00</StartTime>
    <Duration>45</Duration>
    <BarberName>Todd</BarberName>
    <Services>HAIRCUT|SHAVE</Services>
</Appointment>
<Appointment>
    <CustomerName>Brown & Associates</CustomerName>
    <Date>2024-03-16</Date>
    <StartTime>14:00</StartTime>
    <Duration>60</Duration>
    <BarberName>Gerrit</BarberName>
    <Services>FADE|BEARD</Services>
</Appointment>";

        _mockFileReader.ReadAllTextAsync(Arg.Any<string>()).Returns(brokenXml);

        // Act
        var result = await _fixer.ImportAsync("test.xml");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Successes.Count);
        Assert.Empty(result.Failures);
        
        Assert.Equal("Smith & Jones", result.Successes[0].CustomerName);
        Assert.Equal(new DateOnly(2024, 3, 15), result.Successes[0].Date);
        
        Assert.Equal("Brown & Associates", result.Successes[1].CustomerName);
        Assert.Equal(new DateOnly(2024, 3, 16), result.Successes[1].Date);
    }
}
