using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class SelectedBusinessTests
{
    private SelectedBusinessModel? _systemUnderTest;
    protected Mock<ILogger<SelectedBusinessModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new SelectedBusinessModel(_mockLogger.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_TradePartyId_ShouldBeSetToId()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        await _systemUnderTest!.OnGetAsync(id);

        //Assert
        _systemUnderTest.OrgId.Should().Be(id);
    }

    [Test]
    public async Task OnGet_Set_SelectedBusiness_To_BusinessNameFromApi()
    {
        //ARrange
        var tradePartyDto = new TradePartyDto { Id = Guid.NewGuid(), PracticeName = "Test name" };
        _mockTraderService
            .Setup(action => action.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyDto);

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>());

        //Assert
        _systemUnderTest.SelectedBusinessName.Should().Be(tradePartyDto.PracticeName);
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
