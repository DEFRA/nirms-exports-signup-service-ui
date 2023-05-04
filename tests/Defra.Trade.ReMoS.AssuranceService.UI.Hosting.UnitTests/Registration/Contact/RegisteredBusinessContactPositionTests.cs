using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessContactPositionTests : PageModelTestsBase
{
    private RegisteredBusinessContactPositionModel _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessContactPositionModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessContactPositionModel(_mockLogger.Object);
    }

    [Test]
    public async Task OnGet_NoPositionPresentIfNoSavedData()
    {
        // arrange
        // TODO add setup for returning values when api referenced

        // act
        await _systemUnderTest.OnGetAsync();

        // assert
        _systemUnderTest.Position.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange
        _systemUnderTest.Position = "aZ9 -'";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidCharacterInformation()
    {
        // arrange
        _systemUnderTest.Position = "*";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitTooManyCharactersInformation()
    {
        // arrange
        _systemUnderTest.Position = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(1);
    }
}
