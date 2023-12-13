using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe;

[TestFixture]
public class SelfServeDashboardTests : PageModelTestsBase
{
    private SelfServeDashboardModel? _systemUnderTest;
    private readonly Mock<ITraderService> _mockTraderService = new();
    private readonly Mock<IEstablishmentService> _mockEstablishmentService = new();
    private readonly ICheckAnswersService _checkAnswersService = new CheckAnswersService();
    protected Mock<ILogger<SelfServeDashboardModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new SelfServeDashboardModel(_mockLogger.Object, _mockTraderService.Object, _mockEstablishmentService.Object, _checkAnswersService)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
    }

    [Test]
    public async Task OnGet_BusinessNameAndRmsNumberSetToValidValues_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.NewGuid();
        TradePartyDto tradePartyDto = new()
        {
            Id = guid,
            PracticeName = "TestPractice",
            RemosBusinessSchemeNumber = "RMS-GB-000002"
        };
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.RegistrationID.Should().Be(guid);
        _systemUnderTest.BusinessName.Should().Be("TestPractice");
        _systemUnderTest.RmsNumber.Should().Be("RMS-GB-000002");
    }

    [Test]
    public async Task OnGet_BusinessNameAndRmsNumberSetToNull_IfNoSavedData()
    {
        //Arrange
        Guid guid = Guid.NewGuid();
        TradePartyDto tradePartyDto = null!;
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.RegistrationID.Should().Be(guid);
        _systemUnderTest.BusinessName.Should().BeNullOrEmpty();
        _systemUnderTest.RmsNumber.Should().BeNullOrEmpty();
    }

}
