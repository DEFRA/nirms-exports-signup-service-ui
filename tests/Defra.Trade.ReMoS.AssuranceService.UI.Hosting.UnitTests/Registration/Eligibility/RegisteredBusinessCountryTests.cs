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
    private readonly Mock<ICheckAnswersService> _mockCheckAnswersService = new();
    private RegisteredBusinessCountryModel? _systemUnderTest;    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessCountryModel(_mockLogger.Object, _mockTraderService.Object, _mockCheckAnswersService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
    }

    [Test]
    public async Task OnGet_NoCountryPresentIfNoSavedData()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced
        Guid guid = Guid.Empty;

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
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Assert
        _systemUnderTest.CountrySaved.Should().Be(false);
    }

    [Test]
    public async Task OnGet_CountrySavedSetToTrue_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDto();
        var tradeAddress = new TradeAddressDto { TradeCountry = "GB"};

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

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

        var tradeContact = new TradeContactDto();
        var tradeAddress = new TradeAddressDto { TradeCountry = "NI" };

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

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

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.GBChosen = "send";
        _systemUnderTest!.Country = "";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert            
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidFirstInput()
    {
        //Arrange
        _systemUnderTest!.CountrySaved = false;
        _systemUnderTest!.GBChosen = null;

        //Act
        await _systemUnderTest.OnPostSubmitAsync();

        //Assert            
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnGet_IfNoSavedData_ReturnTradePartyDto()
    {
        //Arrange
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDto();
        var tradeAddress = new TradeAddressDto();
        tradeAddress.TradeCountry = "England";

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        _ = await _systemUnderTest!.OnGetAsync(guid);

        //Assert
        _ = _systemUnderTest.Country.Should().Be("England");
    }

    [Test]
    public async Task OnPost_IfCountrySaved_ReturnRedirectToPage()
    {
        //Arrange
        _systemUnderTest!.CountrySaved = true;

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        //Assert
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Test]
    public async Task OnPost_IfModelValid_ReturnRedirectToFboNumberPage()
    {
        //Arrange
        _systemUnderTest!.CountrySaved = false;
        _systemUnderTest!.GBChosen = "send";
        _systemUnderTest.Country = "GB";
        var traderId = Guid.NewGuid();
        _systemUnderTest.TraderId = traderId;
        _mockTraderService
            .Setup(action => action.AddTradePartyAddressAsync(It.IsAny<Guid>(), It.IsAny<TradeAddressDto>()))
            .ReturnsAsync(traderId);

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        ((RedirectToPageResult)result!).PageName.Should().Be(Routes.Pages.Path.RegisteredBusinessFboNumberPath);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGetAsync_RedirectRegisteredBusiness()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
    }
}


