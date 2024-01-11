using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Eligibility;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Eligibility;

[TestFixture]
public class RegisteredBusinessCountryStaticTests : PageModelTestsBase
{
    protected Mock<ILogger<RegisteredBusinessCountryStaticModel>> _mockLogger = new();
    private readonly Mock<ITraderService> _mockTraderService = new();
    private RegisteredBusinessCountryStaticModel? _systemUnderTest;

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessCountryStaticModel(_mockLogger.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(false);
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf") });
    }

    [Test]
    public async Task OnGet_ModelPropertiesNotPopulated()
    {
        // arrange
        Guid guid = Guid.Empty;

        // act
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Empty });
        await _systemUnderTest!.OnGetAsync(guid);

        // assert
        _systemUnderTest.Country.Should().Be("");
        _systemUnderTest.ContentText.Should().Be("");
        _systemUnderTest.TradePartyId.Should().Be(Guid.Empty);
    }

    [Test]
    public async Task OnGet_ValidateOrdId_FalseRedirects()
    {
        // arrange
        var guid = Guid.NewGuid();
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

        // act
        var result = await _systemUnderTest!.OnGetAsync(guid);

        // assert
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGet_IsTradePartySignedUp_TrueRedirects()
    {
        // arrange
        var guid = Guid.NewGuid();
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

        // act
        var result = await _systemUnderTest!.OnGetAsync(guid);

        // assert
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
    }

    [Test]
    public async Task OnGet_GbPopulatesModelVariables()
    {
        // arrange
        var guid = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
        var tradeParty = new TradePartyDto()
        {
            Id = guid,
            Address = new TradeAddressDto()
            {
                TradeCountry = "England"
            }
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest!.OnGetAsync(guid);

        // assert
        _systemUnderTest.Country.Should().Be("England");
        _systemUnderTest.ContentText.Should().Be("sending");
    }

    [Test]
    public async Task OnGet_NiPopulatesModelVariables()
    {
        // arrange
        var guid = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
        var tradeParty = new TradePartyDto()
        {
            Id = guid,
            Address = new TradeAddressDto()
            {
                TradeCountry = "NI"
            }
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest!.OnGetAsync(guid);

        // assert
        _systemUnderTest.Country.Should().Be("NI");
        _systemUnderTest.ContentText.Should().Be("receiving");
    }
}
