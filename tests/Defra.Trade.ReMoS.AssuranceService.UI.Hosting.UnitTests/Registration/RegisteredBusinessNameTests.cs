using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

public class RegisteredBusinessNameTests : PageModelTestsBase
{
    private RegisteredBusinessNameModel _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessNameModel>> _mockLogger = new();

    public RegisteredBusinessNameTests()
    {
        _systemUnderTest = new RegisteredBusinessNameModel(_mockLogger.Object);
    }

    [Fact]
    public async Task OnGet_NoNAmePresentIfNoSavedData()
    {
        // arrange
        // TODO add setup for returning values when api referenced

        // act
        await _systemUnderTest.OnGetAsync();

        // assert
        _systemUnderTest.Name.Should().Be("");
    }

    [Fact]
    public async Task OnPostSubmit_SubmitValudInformation()
    {
        // arrange
        _systemUnderTest.Name = "";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }
}
