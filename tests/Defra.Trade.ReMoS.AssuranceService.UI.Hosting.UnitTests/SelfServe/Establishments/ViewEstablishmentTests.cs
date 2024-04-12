using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class ViewEstablishmentTests : PageModelTestsBase
{
    private ViewEstablishmentModel? _systemUnderTest;
    private readonly Mock<ITraderService> _mockTraderService = new();
    private readonly Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ILogger<ViewEstablishmentModel>> _mockLoger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new ViewEstablishmentModel(
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
            }
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(locationId)).ReturnsAsync(logisticsLocation);

        // act
        await _systemUnderTest!.OnGetAsync(orgId, locationId, NI_GBFlag);

        // assert
        _systemUnderTest.NI_GBFlag.Should().Be(NI_GBFlag);
        _systemUnderTest.BusinessName.Should().Be("business");
        _systemUnderTest.LogisticsLocation.Should().Be(logisticsLocation);
        _systemUnderTest.OrgId.Should().Be(orgId);
        _systemUnderTest.TradePartyId.Should().Be(Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf"));
        _systemUnderTest.ContentText.Should().Be(contentText);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        // arrange
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        // act
        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), "GB");
        var redirectResult = result as RedirectToPageResult;

        // asser
        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }
}
