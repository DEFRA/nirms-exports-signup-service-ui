using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
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
    private TermsAndConditions? _systemUnderTest;
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<IUserService> _mockUserService = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();


    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new TermsAndConditions(_mockTraderService.Object, _mockUserService.Object, _mockEstablishmentService.Object);
    }

    [Test]
    public async Task OnGet_ReturnsId()
    {
        // arrange
        var tradePartyId = Guid.NewGuid();

        //act
        await _systemUnderTest!.OnGetAsync(tradePartyId);

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
    public async Task OnPost_TickedSuccessful_NullDto()
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
            .ReturnsAsync(value: null);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);

        await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    [Test]
    public async Task OnPost_TickedSuccessful_DataPresent()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };
        var assurance = true;
        var logisticsLocationList = new List<LogisticsLocationDto> {new LogisticsLocationDto() };

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);
         _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>()))
            .ReturnsAsync(logisticsLocationList);

        await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    [Test]
    public async Task OnPost_TickedSuccessful_DataNotPresent()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test"
        };
        var assurance = true;
        var logisticsLocationList = new List<LogisticsLocationDto> { new LogisticsLocationDto() };

        //GetDefraOrgBusinessSignupStatus

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, Core.Enums.TradePartySignupStatus.InProgress));


        await _systemUnderTest.OnGetAsync(tradePartyId);

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    public async Task OnGet_TickedSuccessful_DtoIsNull()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        var assurance = true;
        var logisticsLocationList = new List<LogisticsLocationDto> { new LogisticsLocationDto() };

        //GetDefraOrgBusinessSignupStatus

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new TradePartyDto());
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, Core.Enums.TradePartySignupStatus.InProgress));


        await _systemUnderTest.OnGetAsync(tradePartyId);

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

    public async Task OnPost_TickedSuccessful_DtoIsNull()
    {
        //arrange
        var tradePartyId = Guid.NewGuid();
        var assurance = true;
        var logisticsLocationList = new List<LogisticsLocationDto> { new LogisticsLocationDto() };

        //GetDefraOrgBusinessSignupStatus

        //act
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new TradePartyDto());
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, Core.Enums.TradePartySignupStatus.InProgress));


        await _systemUnderTest.OnGetAsync(tradePartyId);

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

        await _systemUnderTest.OnGetAsync(tradePartyId);

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
    }

}


