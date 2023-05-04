using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

public class RegisteredBusinessContactNameTests : PageModelTestsBase
{
    private RegisteredBusinessContactNameModel _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessContactNameModel>> _mockLogger = new();

    public RegisteredBusinessContactNameTests()
    {
        _systemUnderTest = new RegisteredBusinessContactNameModel(_mockLogger.Object);
    }

    [Fact]
    public async Task OnGet_NoNamePresentIfNoSavedData()
    {
        // arrange
        // TODO add setup for returning values when api referenced

        // act
        await _systemUnderTest.OnGetAsync();

        // assert
        _systemUnderTest.Name.Should().Be("");
    }

    [Fact]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(0);
    }

    [Fact]
    public async Task OnPostSubmit_SubmitInvalidCharacterInformation()
    {
        // arrange
        _systemUnderTest.Name = "*";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }

    [Fact]
    public async Task OnPostSubmit_SubmitTooManyCharactersInformation()
    {
        // arrange
        _systemUnderTest.Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }
}
