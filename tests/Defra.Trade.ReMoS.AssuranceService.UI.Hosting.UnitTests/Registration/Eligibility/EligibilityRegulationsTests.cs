using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Eligibility;

[TestFixture]
public class EligibilityRegulationsTests : PageModelTestsBase
{
    protected Mock<ILogger<EligibilityRegulationsModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();
    private EligibilityRegulationsModel? _systemUnderTest;    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EligibilityRegulationsModel(_mockLogger.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf") });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_NoSavedData()
    {
        //arrange
        var expected = Guid.Empty;

        //act
        await _systemUnderTest!.OnGetAsync(Guid.Empty);
        var result = _systemUnderTest.OrgId;

        //assert
        result.Should().Be(expected);

    }

    [Test]
    public async Task OnGet_GuidOnEntryIsKept()
    {
        //arrange
        var expected = Guid.NewGuid();

        //act
        await _systemUnderTest!.OnGetAsync(expected);
        var result = _systemUnderTest.OrgId;

        //assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task OnGet_SetConfirmedTo_SavedTradePartyConfirmedFlag()
    {
        // Arrange
        var tradeId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
        var tradePartyDto = new TradePartyDto { Id = tradeId, RegulationsConfirmed = true };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(tradePartyDto);

        // Act
        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        // Assert
        _systemUnderTest.Confirmed.Should().BeTrue();
    }

    [Test]
    public async Task OnPost_GuidOnEntryIsKept()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.OrgId = expected;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var result = _systemUnderTest.OrgId;

        //assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task OnPostSubmit_GuidFalseError()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.Confirmed = false;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var validation = _systemUnderTest.ModelState.ErrorCount;

        // assert
        validation.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_GuidTrueNoError()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.Confirmed = true;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var validation = _systemUnderTest.ModelState.ErrorCount;

        // assert
        validation.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmit_RedirectToCountryPage()
    {
        // Arrange
        var traderId = Guid.NewGuid();
        var tradePartyDto = new TradePartyDto { Id = traderId};
        _systemUnderTest!.Confirmed = true;
        _systemUnderTest!.TradePartyId = traderId;
        _mockTraderService
            .Setup(action => action.GetTradePartyByIdAsync(traderId))
            .ReturnsAsync(tradePartyDto);
        _mockTraderService
            .Setup(action => action.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(traderId);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = _systemUnderTest!.TradePartyId });

        // Act
        var result = await _systemUnderTest!.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGetAsync_RedirectRegisteredBusiness()
    {
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
    }

}
