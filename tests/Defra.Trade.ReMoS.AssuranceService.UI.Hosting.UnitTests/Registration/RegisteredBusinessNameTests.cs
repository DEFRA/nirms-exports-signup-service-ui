﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessNameTests : PageModelTestsBase
{
    private RegisteredBusinessNameModel? _systemUnderTest;
    private Mock<ITraderService> _mockTraderService = new();
    protected Mock<ILogger<RegisteredBusinessNameModel>> _mockLogger = new();    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessNameModel(_mockLogger.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
    }

    [Test]
    public async Task OnGet_NoNamePresentIfNoSavedData()
    {
        //Arrange
        //TODO: Add setup for returning values when API referenced
        Guid test = Guid.NewGuid();
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnGetAsync(test);

        //Assert
        _systemUnderTest.Name.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidName()
    {
        //Arrange
        _systemUnderTest.Name = "Business-Name1";            

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);            
    }

    [Test]
    public async Task OnPostSave_SubmitValidName()
    {
        //Arrange
        _systemUnderTest.Name = "Business-Name1";

        //Act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInValidNameNotPresent()
    {
        //Arrange
        _systemUnderTest.Name = "";
        var expectedResult = "Enter your business name";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSave_SubmitInValidNameNotPresent()
    {
        //Arrange
        _systemUnderTest.Name = "";
        var expectedResult = "Enter your business name";

        //Act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidRegex()
    {
        //Arrange
        _systemUnderTest.Name = "Business%%Name1";
        var expectedResult = "Enter your business name using only letters, numbers, parentheses, full stops, commas, undescores, forward slashes, hyphens or apostrophes";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSave_SubmitInvalidRegex()
    {
        //Arrange
        _systemUnderTest.Name = "Business%%Name1";
        var expectedResult = "Enter your business name using only letters, numbers, parentheses, full stops, commas, undescores, forward slashes, hyphens or apostrophes";

        //Act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidLength()
    {
        //Arrange
        _systemUnderTest.Name = new string('a', 101);
        var expectedResult = "Business name is too long";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSave_SubmitInvalidLength()
    {
        //Arrange
        _systemUnderTest.Name = new string('a', 101);
        var expectedResult = "Business name is too long";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.Name = "";
        var expectedResult = "Enter your business name";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnPostSave_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.Name = "";
        var expectedResult = "Enter your business name";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnGet_NoNamePresentIfNoSavedData_ReturnTradeParty()
    {
        //Arrange
        var guid = new Guid();

        var tradeContact = new TradeContactDto();

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnGetAsync(guid);
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }
}
