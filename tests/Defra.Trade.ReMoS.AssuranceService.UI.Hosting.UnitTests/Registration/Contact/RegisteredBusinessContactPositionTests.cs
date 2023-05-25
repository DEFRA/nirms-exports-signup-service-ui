using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Moq;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessContactPositionTests : PageModelTestsBase
{
    private RegisteredBusinessContactPositionModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessContactPositionModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessContactPositionModel(
            _mockLogger.Object,
            _mockTraderService.Object);
    }

    [Test]
    public async Task OnGet_NoPositionPresentIfNoSavedData()
    {
        // arrange
        Guid test = Guid.NewGuid();

        // act
        await _systemUnderTest.OnGetAsync(test);

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

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.Position = "";
        var expectedResult = "Enter the position of the contact person";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");


        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnGet_NoPositionPresentIfNoSavedData_ReturnTradePartyDto()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDTO();
        var tradeAddress = new TradeAddressDTO();

        var tradePartyDto = new TradePartyDTO
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _systemUnderTest.Position.Should().Be("");
    }
}
