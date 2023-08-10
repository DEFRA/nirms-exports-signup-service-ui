using Castle.Core.Logging;
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

#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessAddressTests : PageModelTestsBase
{
    protected Mock<ILogger<RegisteredBusinessAddressModel>> _mockLogger = new ();
    protected Mock<ITraderService> _mockTraderService = new();
    private RegisteredBusinessAddressModel? _systemUnderTest;
    private PageModelMockingUtils pageModelMockingUtils = new PageModelMockingUtils();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessAddressModel(
            _mockLogger.Object, 
            _mockTraderService.Object);

        _systemUnderTest.PageContext = pageModelMockingUtils.MockPageContext();
    }

    [Test]
    public async Task OnGet_NoAddressPresentIfNoSavedData()
    {
        // arrange
        //TODO: add setup for returning value when api reference
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
        // act
        await _systemUnderTest.OnGetAsync();

        // assert
        _systemUnderTest.LineOne.Should().Be("");
        _systemUnderTest.LineTwo.Should().Be("");
        _systemUnderTest.CityName.Should().Be("");
        _systemUnderTest.PostCode.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "Line 1 - '";
        _systemUnderTest.LineTwo = "Line 2 - '";
        _systemUnderTest.CityName = "City - '";
        _systemUnderTest.PostCode = "SW1W 0NY";

        // act 
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSave_SubmitValidInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "Line 1 - '";
        _systemUnderTest.LineTwo = "Line 2 - '";
        _systemUnderTest.CityName = "City - '";
        _systemUnderTest.PostCode = "SW1W 0NY";

        // act 
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    public async Task OnPostSubmit_SubmitInvalidCharacterInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "*";
        _systemUnderTest.LineTwo = "*";
        _systemUnderTest.CityName = "*";
        _systemUnderTest.PostCode = "*";

        // act 
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(4);
    }

    public async Task OnPostSave_SubmitInvalidCharacterInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "*";
        _systemUnderTest.LineTwo = "*";
        _systemUnderTest.CityName = "*";
        _systemUnderTest.PostCode = "*";

        // act 
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(4);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.LineOne = "";
        var expectedResultOne = "Enter address line 1";
        var expectedResultTwo = "Enter a town or city";
        var expectedResultThree = "Enter a postcode";

        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(3);
        expectedResultOne.Should().Be(validation[0].ErrorMessage);
        expectedResultTwo.Should().Be(validation[1].ErrorMessage);
        expectedResultThree.Should().Be(validation[2].ErrorMessage);
    }

    [Test]
    public async Task OnPostSave_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.LineOne = "";
        var expectedResultOne = "Enter address line 1";
        var expectedResultTwo = "Enter a town or city";
        var expectedResultThree = "Enter a postcode";

        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(3);
        expectedResultOne.Should().Be(validation[0].ErrorMessage);
        expectedResultTwo.Should().Be(validation[1].ErrorMessage);
        expectedResultThree.Should().Be(validation[2].ErrorMessage);
    }

    [Test]
    public async Task OnGet_NoAddressPresentIfNoSavedData_GetTradeParty()
    {
        //Arrange
        var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d191");

        var tradeContact = new TradeContactDto();
        var tradeAddress = new TradeAddressDto
        {
            TradeCountry = "Test Country",
            LineOne = "1 Test Lane",
            PostCode = "EC1N 2PB"
        };

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact, 
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest.OnGetAsync(guid);
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);

        // assert
        _systemUnderTest.LineOne.Should().Be("1 Test Lane");
        _systemUnderTest.PostCode.Should().Be("EC1N 2PB");
    }

    [Test]
    public async Task OnPostSubmit_GivenValidGuid_SubmitValidInput()
    {
        // arrange 
        _systemUnderTest.LineOne = "Line 1 - '";
        _systemUnderTest.LineTwo = "Line 2 - '";
        _systemUnderTest.CityName = "City - '";
        _systemUnderTest.PostCode = "SW1W 0NY";

        // act 
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSave_GivenValidGuid_SubmitValidInput()
    {
        // arrange 
        _systemUnderTest.LineOne = "Line 1 - '";
        _systemUnderTest.LineTwo = "Line 2 - '";
        _systemUnderTest.CityName = "City - '";
        _systemUnderTest.PostCode = "SW1W 0NY";

        // act 
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
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

}
