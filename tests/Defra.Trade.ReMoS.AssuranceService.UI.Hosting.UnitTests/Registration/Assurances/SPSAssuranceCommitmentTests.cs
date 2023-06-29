using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
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
    private SPSAssuranceCommitmentsModel? _systemUnderTest;
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new SPSAssuranceCommitmentsModel(_mockTraderService.Object);
    }

    [Test]
    public async Task OnGet_ReturnsId()
    {
        // arrange
        var tradePartyId = Guid.NewGuid();

        //act
        _systemUnderTest!.OnGet(tradePartyId);

        //assert
        _systemUnderTest.TraderId.Should().Be(tradePartyId);
    }

    [Test]
    public async Task OnPost_NotTicked()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDTO tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };
        var assurance = false;

        //act
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
    }

    [Test]
    public async Task OnPost_TickedSuccessful()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDTO tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };
        var assurance = true;

        //act
        _systemUnderTest!.AssuranceCommitment = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDTO>()))
            .ReturnsAsync(tradePartyId);

        await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

}


