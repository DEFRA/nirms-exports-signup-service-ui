using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Assurances;

public class SPSAssuranceCommitmentTests : PageModelTestsBase
{
    private TermsAndConditions? _systemUnderTest;
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<IUserService> _mockUserService = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    private PageModelMockingUtils pageModelMockingUtils = new PageModelMockingUtils();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new TermsAndConditions(_mockTraderService.Object, _mockUserService.Object, _mockEstablishmentService.Object);
        _systemUnderTest.PageContext = pageModelMockingUtils.MockPageContext();
    }

    [Test]
    public void OnGet_ReturnsId()
    {
        // arrange
        var tradePartyId = Guid.NewGuid();

        //act
        _systemUnderTest!.OnGetAsync(tradePartyId);

        //assert
        _systemUnderTest.TraderId.Should().Be(tradePartyId);
    }

    [Test]
    public async Task OnPost_NotTicked()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };

        //act
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        await _systemUnderTest!.OnPostSubmitAsync();

        //assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnPost_TickedSuccessful()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };
        var assurance = true;

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);

        await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    [Test]
    public async Task OnGet_OrgCompletedSuccessful()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };

        var assurance = true;

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, Core.Enums.TradePartySignupStatus.Complete));
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        await _systemUnderTest.OnGetAsync(tradePartyId);

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

}


