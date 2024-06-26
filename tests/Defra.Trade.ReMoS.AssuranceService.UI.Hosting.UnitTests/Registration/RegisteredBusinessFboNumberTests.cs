﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

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
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_IfNoSavedParty_FboNumberShouldBeNull()
    {
        //Arrange
        var orgId = Guid.NewGuid();

        //Act
        await _systemUnderTest!.OnGetAsync(orgId);

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
            FboPhrOption = "fbo",
            PracticeName = "party"
        };
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyFromApi);

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert
        _systemUnderTest.FboNumber.Should().Be("fbonum-123456-fbonum");
        _systemUnderTest.OptionSelected.Should().Be("fbo");
        _systemUnderTest.PracticeName.Should().Be("party");
    }

    [Test]
    public async Task OnGet_IfSavedPartyExists_PhrNumberShouldBePopulated()
    {
        //Arrange
        TradePartyDto tradePartyFromApi = new TradePartyDto
        {
            Id = Guid.NewGuid(),
            PhrNumber = "123123",
            FboPhrOption = "phr"
        };
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyFromApi);

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert
        _systemUnderTest.PhrNumber.Should().Be("123123");
        _systemUnderTest.OptionSelected.Should().Be("phr");
    }

    [Test]
    public async Task OnGet_IfSavedPartyExists_ButNoFboNumberSaved_OptionSelectedShouldBeNoney()
    {
        //Arrange
        TradePartyDto tradePartyFromApi = new TradePartyDto
        {
            Id = Guid.NewGuid(),
            FboPhrOption = "none"
        };
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyFromApi);

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert
        //_systemUnderTest.FboNumber.Should().BeNull();
        _systemUnderTest.OptionSelected.Should().Be("none");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidName()
    {
        //Arrange
        _systemUnderTest!.FboNumber = "fbonum-123456-fbonum";
        _systemUnderTest.OptionSelected = "fbo";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
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
    public async Task OnPostSubmit_SubmitInvalidLength()
    {
        //Arrange
        _systemUnderTest!.OptionSelected = "fbo";
        _systemUnderTest!.FboNumber = new string('1', 26);
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var expectedResult = "FBO number must be 25 characters or less";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSubmit_SubmitNoOption()
    {
        //Arrange
        _systemUnderTest!.OptionSelected = "";
        _systemUnderTest!.PracticeName = "Test";
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var expectedResult = "Select if your business has an FBO or PHR number";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        _systemUnderTest!.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be(expectedResult);
    }

    [Test]
    public async Task OnPostSubmit_SubmitPHRInvalidLength()
    {
        //Arrange
        _systemUnderTest!.OptionSelected = "phr";
        _systemUnderTest!.PhrNumber = new string('1', 26);
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var expectedResult = "PHR number must be 25 characters or less";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnGetAsync_RedirectRegisteredBusiness()
    {
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
    }

    [Test]
    public async Task OnPostSubmitAsync_RedirectToGuidence_IfSelectedNone()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "none";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        // Act
        var result = await _systemUnderTest!.OnPostSubmitAsync();
        var redirectResult = result as RedirectToPageResult;

        // Assert
        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessFboPhrGuidance");
    }

    [Test]
    public async Task OnPostSubmitAsync_RedirectToContactName_IfValidFboPhrGiven()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "fbo";
        _systemUnderTest!.FboNumber = "fbonum-123456-fbonum";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        // Act
        var result = await _systemUnderTest!.OnPostSubmitAsync();
        var redirectResult = result as RedirectToPageResult;

        // Assert
        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/Contact/RegisteredBusinessContactName");
    }

    [Test]
    public async Task OnPostSaveAsync_RedirectToGuidence_IfSelectedNone()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "none";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        // Act
        var result = await _systemUnderTest!.OnPostSaveAsync();
        var redirectResult = result as RedirectToPageResult;

        // Assert
        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessFboPhrGuidance");
    }

    [Test]
    public async Task OnPostSaveAsync_RedirectToTaskList_IfValidFboPhrGiven()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "fbo";
        _systemUnderTest!.FboNumber = "fbonum-123456-fbonum";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        // Act
        var result = await _systemUnderTest!.OnPostSaveAsync();
        var redirectResult = result as RedirectToPageResult;

        // Assert
        redirectResult!.PageName.Should().Be("/Registration/TaskList/RegistrationTaskList");
    }

    [Test]
    public async Task OnPostSaveAsync_SubmitInvalidFbo()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "fbo";
        _systemUnderTest!.FboNumber = string.Empty;
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var expectedResult = "Enter your business's FBO number";

        // Act
        var result = await _systemUnderTest!.OnPostSaveAsync();

        // Assert
        _systemUnderTest.ModelState.Count.Should().Be(1);
        _systemUnderTest!.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be(expectedResult);
    }

    [Test]
    public async Task OnPostSaveAsync_SubmitInvalidPhr()
    {
        // Arrange
        _systemUnderTest!.OptionSelected = "phr";
        _systemUnderTest!.PhrNumber = string.Empty;
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var expectedResult = "Enter your business's PHR number";

        // Act
        var result = await _systemUnderTest!.OnPostSaveAsync();

        // Assert
        _systemUnderTest.ModelState.Count.Should().Be(1);
        _systemUnderTest!.ModelState.Values.First().Errors[0].ErrorMessage.Should().Be(expectedResult);
    }
}
