using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

public class RegisteredBusinessCountryTests : PageModelTestsBase
{
    private RegisteredBusinessCountryModel _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessCountryModel>> _mockLogger = new();

    public RegisteredBusinessCountryTests()
    {
        _systemUnderTest = new RegisteredBusinessCountryModel(_mockLogger.Object);
    }

    [Fact]
    public async Task OnGet_NoCountryPresentIfNoSavedData()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced

        //Act
        await _systemUnderTest.OnGetAsync();

        //Assert
        _systemUnderTest.Country.Should().Be("");
    }

    [Fact]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        //Arrange
        _systemUnderTest.Country = "";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(1);
    }
}


