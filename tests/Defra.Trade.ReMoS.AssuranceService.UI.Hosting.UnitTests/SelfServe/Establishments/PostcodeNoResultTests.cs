using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class PostcodeNoResultTests : PageModelTestsBase
{
    private PostcodeNoResultModel? _systemUnderTest;
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<ILogger<PostcodeNoResultModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new PostcodeNoResultModel(_mockTraderService.Object, _mockLogger.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
    }

    [Test]
    public async Task OnGet_PopulateModelPopertiesNI()
    {
        // arrange

        // act
        await _systemUnderTest!.OnGet(Guid.NewGuid(), "NI", "TES1");

        // assert
        _systemUnderTest.ContentCountry.Should().Be("Northern Ireland");
        _systemUnderTest.ContentHeading.Should().Be("Add a place of destination");
        _systemUnderTest.ContentText.Should().Be("The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.");

    }

    [Test]
    public async Task OnGet_PopulateModelPopertiesGB()
    {
        // arrange

        // act
        await _systemUnderTest!.OnGet(Guid.NewGuid(), "GB", "TES1");

        // assert
        _systemUnderTest.ContentCountry.Should().Be("England, Scotland and Wales");
        _systemUnderTest.ContentHeading.Should().Be("Add a place of dispatch");
        _systemUnderTest.ContentText.Should().Be("The locations which are part of your business that consignments to Northern Ireland will depart from under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.");

    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGet(Guid.NewGuid(), "GB", "TES1");
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }


}
