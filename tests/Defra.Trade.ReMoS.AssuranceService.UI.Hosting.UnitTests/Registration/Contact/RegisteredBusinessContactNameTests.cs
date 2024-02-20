using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessContactNameTests : PageModelTestsBase
{
    private RegisteredBusinessContactNameModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessContactNameModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();    

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessContactNameModel(_mockLogger.Object, _mockTraderService.Object)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_NoNamePresentIfNoSavedData()
    {
        // arrange
        Guid test = Guid.NewGuid();

        // act
        await _systemUnderTest.OnGetAsync(test);

        // assert
        _systemUnderTest.Name.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";

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
        _systemUnderTest.Name = "John Doe";
        _systemUnderTest.PracticeName = "ACME Ltd";
        var tradeParty = new TradePartyDto
        {
            Contact = new TradeContactDto
            {
                IsAuthorisedSignatory = true,
            }
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidCharacterInformation()
    {
        // arrange
        _systemUnderTest.Name = "*";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSave_SubmitInvalidCharacterInformation()
    {
        // arrange
        _systemUnderTest.Name = "*";
        _systemUnderTest.TradePartyId = Guid.NewGuid();

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // asser
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitTooManyCharactersInformation()
    {
        // arrange
        _systemUnderTest.Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        // act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSave_SubmitTooManyCharactersInformation()
    {
        // arrange
        _systemUnderTest.Name = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_SubmitInvalidInput()
    {
        //Arrange
        _systemUnderTest!.Name = "";
        var expectedResult = "Enter a name";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

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
        var expectedResult = "Enter a name";
        _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

        //Act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert            
        validation.Count.Should().Be(1);
        expectedResult.Should().Be(validation[0].ErrorMessage);
    }

    [Test]
    public async Task OnGet_NoNamePresentIfNoSavedData_ReturnTradePartyDto()
    {
        //Arrange
        Guid guid = Guid.NewGuid();

        var tradeContact = new TradeContactDto();
        var tradeAddress = new TradeAddressDto();

        var tradePartyDto = new TradePartyDto
        {
            Id = guid,
            Contact = tradeContact,
            Address = tradeAddress
        };

        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert
        _mockTraderService.Verify();
        _systemUnderTest.Name.Should().Be("");
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

    [Test]
    public async Task OnPostSave_PopulateAuthRepWhenChangeName()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";
        _systemUnderTest.PracticeName = "ACME Ltd";
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var tradeParty = new TradePartyDto
        {
            Contact = new TradeContactDto
            {
                IsAuthorisedSignatory = true,
            },
            AuthorisedSignatory = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
                Name = "John Doe Old"
            }
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
        _systemUnderTest.AuthorisedSignatoryId.Should().Be(tradeParty.AuthorisedSignatory.Id);
        _systemUnderTest.TradePartyDto.Contact.PersonName.Should().Be("John Doe");
        _systemUnderTest.TradePartyDto.Contact.PersonName.Should().Be(_systemUnderTest.TradePartyDto.AuthorisedSignatory.Name);
        _systemUnderTest.AuthorisedSignatoryId.Should().Be(_systemUnderTest.TradePartyDto.AuthorisedSignatory.Id);
    }

    [Test]
    public async Task OnPostSave_NotPopulateAuthRepWhenChangeName()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";
        _systemUnderTest.PracticeName = "ACME Ltd";
        //_systemUnderTest.TradePartyId = Guid.NewGuid();
        var tradeParty = new TradePartyDto
        {
            Contact = new TradeContactDto
            {
                IsAuthorisedSignatory = false,
            },
            AuthorisedSignatory = new AuthorisedSignatoryDto
            {
                Id = Guid.NewGuid(),
                Name = "John Doe Old"
            }
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
        _systemUnderTest.AuthorisedSignatoryId.Should().Be(tradeParty.AuthorisedSignatory.Id);
        _systemUnderTest.TradePartyDto.Contact.PersonName.Should().Be("John Doe");
        _systemUnderTest.TradePartyDto.AuthorisedSignatory.Should().BeNull();
    }

    [Test]
    public async Task OnPostSave_NotCompletedAuthRep()
    {
        // arrange
        _systemUnderTest.Name = "John Doe";
        _systemUnderTest.PracticeName = "ACME Ltd";
        _systemUnderTest.TradePartyId = Guid.NewGuid();
        var tradeParty = new TradePartyDto
        {
            Contact = new TradeContactDto
            {
                IsAuthorisedSignatory = null,
            }
        };
        _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradeParty);

        // act
        await _systemUnderTest.OnPostSaveAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
        _systemUnderTest.AuthorisedSignatoryId.Should().Be(Guid.Empty);
        _systemUnderTest.TradePartyDto.Contact.PersonName.Should().Be("John Doe");
        _systemUnderTest.TradePartyDto.AuthorisedSignatory.Should().BeNull();
    }
}
