using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessFboNumberTests : PageModelTestsBase
{
    private RegisteredBusinessFboNumberModel? _systemUnderTest;
    private Mock<ITraderService> _mockTraderService = new();
    protected Mock<ILogger<RegisteredBusinessFboNumberModel>> _mockLogger = new();    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessFboNumberModel(_mockLogger.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
    }

    [Test]
    public async Task OnGet_IfNoSavedParty_FboNumberShouldBeNull()
    {
        //Arrange
        var tradePartyId = Guid.Empty;
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest!.OnGetAsync(tradePartyId);

        //Assert
        _systemUnderTest.FboNumber.Should().Be(string.Empty);
    }

    [Test]
    public async Task OnGet_IfSavedPartyExists_FboNumberShouldBePopulated()
    {
        //Arrange
        TradePartyDto tradePartyFromApi = new TradePartyDto
        {
            Id = Guid.NewGuid(),
            FboNumber = "fbonum-123456-fbonum",
        };
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyFromApi);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest!.OnGetAsync(tradePartyFromApi.Id);

        //Assert
        _systemUnderTest.FboNumber.Should().Be("fbonum-123456-fbonum");
        _systemUnderTest.OptionSelected.Should().Be("yes");
    }

    [Test]
    public async Task OnGet_IfSavedPartyExists_ButNoFboNumberSaved_OptionSelectedShouldBeEmpty()
    {
        //Arrange
        TradePartyDto tradePartyFromApi = new TradePartyDto
        {
            Id = Guid.NewGuid(),
            //FboNumber = "fbonum-123456-fbonum",
        };
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyFromApi);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest!.OnGetAsync(tradePartyFromApi.Id);

        //Assert
        //_systemUnderTest.FboNumber.Should().BeNull();
        _systemUnderTest.OptionSelected.Should().Be(string.Empty);
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidName()
    {
        //Arrange
        _systemUnderTest!.FboNumber = "fbonum-123456-fbonum";
        _systemUnderTest.OptionSelected = "yes";

        //Act
        await _systemUnderTest.OnPostAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }


    [Test]
    public async Task OnPostSubmit_SubmitInvalidLength()
    {
        //Arrange
        _systemUnderTest!.OptionSelected = "yes";
        _systemUnderTest!.FboNumber = new string('1', 26);
        var expectedResult = "FBO number is too long";

        //Act
        await _systemUnderTest.OnPostAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }
}
