using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessCountryTests : PageModelTestsBase
{
    protected Mock<ILogger<RegisteredBusinessCountryModel>> _mockLogger = new();
    private RegisteredBusinessCountryModel? _systemUnderTest;

    [Test]
    public async Task OnGet_NoCountryPresentIfNoSavedData()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced
        _systemUnderTest = new RegisteredBusinessCountryModel(_mockLogger.Object);

        //Act
        _ = await _systemUnderTest.OnGetAsync();

        //Assert
        _ = _systemUnderTest.Country.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        //Arrange
        _systemUnderTest = new RegisteredBusinessCountryModel(_mockLogger.Object);
        _systemUnderTest.Country = "";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        _ = validation.Count.Should().Be(1);
    }
}


