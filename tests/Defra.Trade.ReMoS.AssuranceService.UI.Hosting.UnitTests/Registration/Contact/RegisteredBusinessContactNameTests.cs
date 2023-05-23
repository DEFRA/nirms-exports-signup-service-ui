using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessContactNameTests : PageModelTestsBase
{
    private RegisteredBusinessContactNameModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessContactNameModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessContactNameModel(_mockLogger.Object, _mockTraderService.Object);
    }

    [Test]
    public async Task OnGet_NoNamePresentIfNoSavedData()
    {
        // arrange
        Guid test = Guid.NewGuid();

        // act
        await _systemUnderTest.OnGetAsync(test);

        // assert
        _systemUnderTest.Name.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";

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
        _systemUnderTest.Name = "*";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitTooManyCharactersInformation()
    {
        // arrange
        _systemUnderTest.Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.Name = "";
        var expectedResult = "Enter a name.";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");


        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
    }
}
