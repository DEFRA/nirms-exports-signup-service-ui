using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe;

[TestFixture]
public class EligibilityRegulationsTests : PageModelTestsBase
{
    private EligibilityRegulationsModel? _systemUnderTest;
    protected Mock<ILogger<EligibilityRegulationsModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EligibilityRegulationsModel(
            _mockLogger.Object,
            _mockTraderService.Object,
            _mockEstablishmentService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public void OnGet_SetButtonText_ToNI()
    {
        //Act
        _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), "NI");

        //Assert
        _systemUnderTest.ButtonText.Should().Be("Add place of destination");
    }

    [Test]
    public void OnGet_SetButtonText_ToDispatch()
    {
        //Act
        _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), "GB");

        //Assert
        _systemUnderTest.ButtonText.Should().Be("Add place of dispatch");
    }
    [Test]
    public async Task OnPostSubmit_RedirectSuccessfully()
    {
        // Arrange
        _mockEstablishmentService
            .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(It.IsAny<LogisticsLocationDto>());
        _mockEstablishmentService
            .Setup(x => x.UpdateEstablishmentDetailsSelfServeAsync(It.IsAny<LogisticsLocationDto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _systemUnderTest!.OnPostSubmitAsync();

        // Assert
        result?.GetType().Should().Be(typeof(RedirectToPageResult));
        (result as RedirectToPageResult)?.PageName?.Equals(Routes.Pages.Path.SelfServeEstablishmentAddedPath);
    }

}
