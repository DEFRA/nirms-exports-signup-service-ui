using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Assurances;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.BatchAI.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Assurances;

public class UpdatedTermsAndConditionsTests : PageModelTestsBase
{
    private UpdatedTermsAndConditions? _systemUnderTest;
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<IUserService> _mockUserService = new();
    protected Mock<IConfiguration> _mockConfig = new();
    protected Mock<ILogger<UpdatedTermsAndConditions>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new UpdatedTermsAndConditions(_mockTraderService.Object, _mockUserService.Object, _mockConfig.Object, _mockLogger.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("0f552d9a-6415-4c2a-af04-09a9075ec995") });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGetAsync_ModelPopulated()
    {
        // arrange
        var _mockConfigSection = new Mock<IConfigurationSection>();
        _mockConfigSection.Setup(x => x.Value).Returns("01/02/2023");
        _mockConfig.Setup(x => x.GetSection("UpdatedTermsAndConditionsDate")).Returns(_mockConfigSection.Object);
        var orgId = Guid.NewGuid();
        var tradePartyId = Guid.Parse("0f552d9a-6415-4c2a-af04-09a9075ec995");
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test",
            TermsAndConditionsSignedDate = DateTime.ParseExact("09/04/2004", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Name = "name"
            },
            PracticeName = "test"
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        //act
        await _systemUnderTest!.OnGetAsync(orgId);

        //assert
        _systemUnderTest.OrgId.Should().Be(orgId);
        _systemUnderTest.UpdatedTermsAndConditionsDate.Should().Be("1 February 2023");
        _systemUnderTest.TradePartyId.Should().Be(tradePartyId);
        _systemUnderTest.AuthorisedSignatoryName.Should().Be("name");
        _systemUnderTest.PracticeName.Should().Be("test");
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
            Id = tradePartyId
        };
        var assurance = true;
        _systemUnderTest!.TandCs = assurance;
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.UpdateTradePartyAsync(It.IsAny<TradePartyDto>()))
            .ReturnsAsync(tradePartyId);

        //act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        //assert
        _systemUnderTest!.ModelState.ErrorCount.Should().Be(0);
        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        // arrange
        var _mockConfigSection = new Mock<IConfigurationSection>();
        _mockConfigSection.Setup(x => x.Value).Returns("01/02/2023");
        _mockConfig.Setup(x => x.GetSection("UpdatedTermsAndConditionsDate")).Returns(_mockConfigSection.Object);
        var orgId = Guid.NewGuid();
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test",
            TermsAndConditionsSignedDate = DateTime.ParseExact("01/02/2002", "dd/MM/yyyy", CultureInfo.InvariantCulture),
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        // act
        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        // asser
        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGetAsync_RedirectToDashboardWhenNull()
    {
        // arrange
        var orgId = Guid.NewGuid();
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test",
            TermsAndConditionsSignedDate = DateTime.ParseExact("01/02/2002", "dd/MM/yyyy", CultureInfo.InvariantCulture),
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);
        var _mockConfigSection = new Mock<IConfigurationSection>();
#pragma warning disable CS8603 // Possible null reference return.
        _mockConfigSection.Setup(x => x.Value).Returns(() => null);
#pragma warning restore CS8603 // Possible null reference return.
        _mockConfig.Setup(x => x.GetSection("UpdatedTermsAndConditionsDate")).Returns(_mockConfigSection.Object);

        // act
        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        // assert
        redirectResult!.PageName.Should().Be("/SelfServe/SelfServeDashboard");
    }

    [Test]
    public async Task OnGetAsync_RedirectToDashboardWhenUpdated()
    {
        // arrange
        var _mockConfigSection = new Mock<IConfigurationSection>();
        _mockConfigSection.Setup(x => x.Value).Returns("01/02/2023");
        _mockConfig.Setup(x => x.GetSection("UpdatedTermsAndConditionsDate")).Returns(_mockConfigSection.Object);
        var orgId = Guid.NewGuid();
        var tradePartyId = Guid.NewGuid();
        TradePartyDto tradeParty = new()
        {
            Id = tradePartyId,
            PartyName = "Test",
            TermsAndConditionsSignedDate = DateTime.ParseExact("01/01/2032", "dd/MM/yyyy", CultureInfo.InvariantCulture),
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        // act
        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        // assert
        redirectResult!.PageName.Should().Be("/SelfServe/SelfServeDashboard");
    }
}
