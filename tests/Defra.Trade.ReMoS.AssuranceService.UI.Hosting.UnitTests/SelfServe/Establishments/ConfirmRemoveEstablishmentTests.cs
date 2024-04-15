using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class ConfirmRemoveEstablishmentTests : PageModelTestsBase
{
    private ConfirmRemoveEstablishmentModel? _systemUnderTest;
    private readonly Mock<ITraderService> _mockTraderService = new();
    private readonly Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ILogger<ConfirmRemoveEstablishmentModel>> _mockLoger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new ConfirmRemoveEstablishmentModel(
            _mockLoger.Object,
            _mockEstablishmentService.Object,
            _mockTraderService.Object)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf"), PracticeName = "business" });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [TestCase("GB", "dispatch")]
    [TestCase("NI", "destination")]
    public async Task OnGetAsync_PopulatesModel(string NI_GBFlag, string contentText)
    {
        // arrange
        var orgId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cc");
        var locationId = Guid.NewGuid();
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = Guid.NewGuid(),
            TradePartyId = Guid.NewGuid(),
            Name = "test name",
            RemosEstablishmentSchemeNumber = "remos",
            Email = "test email",
            Address = new TradeAddressDto()
            {
                LineOne = "line 1",
                LineTwo = "lines 2",
                PostCode = "postcode",
                CityName = "city"
            },
            ApprovalStatus = LogisticsLocationApprovalStatus.Approved
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(locationId)).ReturnsAsync(logisticsLocation);

        // act
        await _systemUnderTest!.OnGetAsync(orgId, locationId, NI_GBFlag);

        // assert
        _systemUnderTest.NI_GBFlag.Should().Be(NI_GBFlag);
        
        _systemUnderTest.Establishment.Should().Be(logisticsLocation);
        _systemUnderTest.OrgId.Should().Be(orgId);
        _systemUnderTest.TradePartyId.Should().Be(Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf"));
        _systemUnderTest.DispatchOrDestination.Should().Be(contentText);
    }

    [Test]
    public async Task OnPostSubmitAsync_RedirectToEstablishmentRemovedPage()
    {
        // arrange
        var location = new LogisticsLocationDto() { Id = Guid.NewGuid(), ApprovalStatus = LogisticsLocationApprovalStatus.Approved };
        
        _mockEstablishmentService
            .Setup(action => action.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(location);
        
        _mockEstablishmentService
            .Setup(action => action.UpdateEstablishmentDetailsSelfServeAsync(It.IsAny<LogisticsLocationDto>()))
            .ReturnsAsync(true);

        // act
        var result = await _systemUnderTest!.OnPostSubmitAsync();

        // assert
        result.Should().NotBeNull();
        result.GetType().Should().Be(typeof(RedirectToPageResult));
        ((RedirectToPageResult)result).PageName.Should().Be(Routes.Pages.Path.SelfServeEstablishmentRemovedPath);
    }

    [Test]
    public async Task OnGetAsync_RedirectToEstablishmentErrorPage()
    {
        // arrange
        var orgId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cc");
        var locationId = Guid.NewGuid();
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = Guid.NewGuid(),
            TradePartyId = Guid.NewGuid(),
            Name = "test name",
            RemosEstablishmentSchemeNumber = "remos",
            Email = "test email",
            Address = new TradeAddressDto()
            {
                LineOne = "line 1",
                LineTwo = "lines 2",
                PostCode = "postcode",
                CityName = "city"
            },
            ApprovalStatus = LogisticsLocationApprovalStatus.Removed
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(locationId)).ReturnsAsync(logisticsLocation);

        // act
        var result = await _systemUnderTest!.OnGetAsync(orgId, locationId, "GB");

        // assert
        result.Should().NotBeNull();
        result.GetType().Should().Be(typeof(RedirectToPageResult));
        ((RedirectToPageResult)result).PageName.Should().Be(Routes.Pages.Path.EstablishmentErrorPath);
    }

}
