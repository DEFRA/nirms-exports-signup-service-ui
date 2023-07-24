using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Eligibility;

[TestFixture]
public class RegisteredBusinessCountryTests : PageModelTestsBase
{
    protected Mock<ILogger<RegisteredBusinessCountryModel>> _mockLogger = new();
    private readonly Mock<ITraderService> _mockTraderService = new();
    private RegisteredBusinessCountryModel? _systemUnderTest;

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessCountryModel(_mockLogger.Object, _mockTraderService.Object);
    }

    [Test]
    public async Task OnGet_NoCountryPresentIfNoSavedData()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced
        Guid guid = Guid.NewGuid();

        //Act
        _ = await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _ = _systemUnderTest.Country.Should().Be("");
    }

    [Test]
    public async Task OnGet_CountrySavedSetToFalse_IfNoSavedData()
    {        
        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert
        _systemUnderTest.CountrySaved.Should().Be(false);
    }

    [Test]
    public async Task OnGet_CountrySavedSetToTrue_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDTO();
        var tradeAddress = new TradeAddressDTO { TradeCountry = "GB"};

        var tradePartyDto = new TradePartyDTO
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        _ = await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _ = _systemUnderTest.GBChosen.Should().Be("send");
        _ = _systemUnderTest.Country.Should().Be("GB");
        _ = _systemUnderTest.CountrySaved.Should().Be(true);
    }

    [Test]
    public async Task OnGet_CountrySavedSetToTrueNI_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDTO();
        var tradeAddress = new TradeAddressDTO { TradeCountry = "NI" };

        var tradePartyDto = new TradePartyDTO
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        _ = await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _ = _systemUnderTest.GBChosen.Should().Be("recieve");
        _ = _systemUnderTest.Country.Should().Be("NI");
        _ = _systemUnderTest.CountrySaved.Should().Be(true);
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        //Arrange
        _systemUnderTest!.Country = "";
        Guid guid = Guid.NewGuid();

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        _ = validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.GBChosen = "send";
        _systemUnderTest!.Country = "";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert            
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidFirstInput()
    {
        //Arrange
        _systemUnderTest!.GBChosen = "";
        var expectedResult = "Select what your business will do under the scheme";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");


        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnGet_IfNoSavedData_ReturnTradePartyDto()
    {
        //Arrange
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
        _ = await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _ = _systemUnderTest.Country.Should().Be("");
    }

    [Test]
    public async Task OnPost_IfCountrySaved_ReturnRedirectToPage()
    {
        //Arrange
        _systemUnderTest!.CountrySaved = true;

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        result.Should().BeOfType<PageResult>();
    }

    [Test]
    public async Task OnPost_IfModelValid_ReturnRedirectToFboNumberPage()
    {
        //Arrange
        _systemUnderTest!.CountrySaved = false;
        _systemUnderTest.Country = "GB";
        var traderId = Guid.NewGuid();
        _systemUnderTest.TraderId = traderId;
        _mockTraderService
            .Setup(action => action.AddTradePartyAddressAsync(It.IsAny<Guid>(), It.IsAny<TradeAddressDTO>()))
            .ReturnsAsync(traderId);

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result!).PageName.Should().Be(Routes.Pages.Path.RegisteredBusinessFboNumberPath);
    }
}


