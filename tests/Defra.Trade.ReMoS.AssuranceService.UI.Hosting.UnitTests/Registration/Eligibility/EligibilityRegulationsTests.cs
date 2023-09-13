using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    [Test]
    public async Task OnGet_NoSavedData()
    {
        //arrange
        var expected = Guid.Empty;

        //act
        await _systemUnderTest!.OnGetAsync(Guid.Empty);
        var result = _systemUnderTest.TraderId;

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
        var result = _systemUnderTest.TraderId;

        //assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task OnGet_SetConfirmedTo_SavedTradePartyConfirmedFlag()
    {
        // Arrange
        var tradeId = Guid.NewGuid();
        var tradePartyDto = new TradePartyDto { Id = tradeId, RegulationsConfirmed = true };
        _mockTraderService
            .Setup(action => action.GetTradePartyByIdAsync(tradeId))
            .ReturnsAsync(tradePartyDto);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        // Act
        var result = await _systemUnderTest!.OnGetAsync(tradeId);

        // Assert
        _systemUnderTest.Confirmed.Should().BeTrue();
    }

    [Test]
    public async Task OnPost_GuidOnEntryIsKept()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.TraderId = expected;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var result = _systemUnderTest.TraderId;

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
    public async Task OnPostSubmit_RedirectToTaskList()
    {
        // Arrange
        var traderId = Guid.NewGuid();
        var tradePartyDto = new TradePartyDto { Id = traderId};
        _systemUnderTest!.Confirmed = true;
        _systemUnderTest!.TraderId = traderId;
        _mockTraderService
            .Setup(action => action.GetTradePartyByIdAsync(traderId))
            .ReturnsAsync(tradePartyDto);
        _mockTraderService
            .Setup(action => action.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(traderId);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = _systemUnderTest!.TraderId });

        // Act
        var result = await _systemUnderTest!.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
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
