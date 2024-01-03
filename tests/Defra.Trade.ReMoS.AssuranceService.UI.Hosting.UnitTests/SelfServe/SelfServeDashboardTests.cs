using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

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
        _systemUnderTest = new SelfServeDashboardModel(
            _mockLogger.Object, 
            _mockTraderService.Object, 
            _mockEstablishmentService.Object, 
            _checkAnswersService)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
    }

    [Test]
    public async Task OnGet_ModelPropertiesSetToValidValues_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.NewGuid();
        TradePartyDto tradePartyDto = new()
        {
            Id = guid,
            PracticeName = "TestPractice",
            RemosBusinessSchemeNumber = "RMS-GB-000002",
            Contact = new TradeContactDto()
            {
                PersonName = "Joe Blogs",
                Position = "Sales rep",
                Email = "sd@sd.com",
                TelephoneNumber = "1234567890",
                LastModifiedDate = DateTime.Now,
                SubmittedDate = DateTime.Now,
            },
            Address = new TradeAddressDto()
            {
                TradeCountry = "England"
            },
            AuthorisedSignatory = new AuthorisedSignatoryDto()
            {
                Name = "John Doe",
                Position = "Sales rep",
                EmailAddress = "auth@sd.com",
                LastModifiedDate = DateTime.Now,
                SubmittedDate = DateTime.Now,
            }
        };
        _mockTraderService
            .Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyDto!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.RegistrationID.Should().Be(guid);
        _systemUnderTest.BusinessName.Should().Be("TestPractice");
        _systemUnderTest.RmsNumber.Should().Be("RMS-GB-000002");
        
        _systemUnderTest.ContactName.Should().Be("Joe Blogs");
        _systemUnderTest.ContactPosition.Should().Be("Sales rep");
        _systemUnderTest.ContactEmail.Should().Be("sd@sd.com");
        _systemUnderTest.ContactPhoneNumber.Should().Be("1234567890");
        _systemUnderTest.ContactLastModifiedDate.Should().Be(tradePartyDto.Contact.LastModifiedDate);
        _systemUnderTest.ContactSubmittedDate.Should().Be(tradePartyDto.Contact.SubmittedDate);
        
        _systemUnderTest.AuthSignatoryName.Should().Be("John Doe");
        _systemUnderTest.AuthSignatoryPosition.Should().Be("Sales rep");
        _systemUnderTest.AuthSignatoryEmail.Should().Be("auth@sd.com");
        _systemUnderTest.AuthSignatoryLastModifiedDate.Should().Be(tradePartyDto.AuthorisedSignatory.LastModifiedDate);
        _systemUnderTest.AuthSignatorySubmittedDate.Should().Be(tradePartyDto.AuthorisedSignatory.SubmittedDate);
    }

    [Test]
    public async Task OnGet_ModelProperties_SetToNull_IfNoSavedData()
    {
        //Arrange
        Guid guid = Guid.NewGuid();
        TradePartyDto tradePartyDto = null!;
        _mockTraderService
            .Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.RegistrationID.Should().Be(guid);
        _systemUnderTest.BusinessName.Should().BeNullOrEmpty();
        _systemUnderTest.RmsNumber.Should().BeNullOrEmpty();
        
        _systemUnderTest.ContactName.Should().BeNullOrEmpty();
        _systemUnderTest.ContactPosition.Should().BeNullOrEmpty();
        _systemUnderTest.ContactEmail.Should().BeNullOrEmpty();
        _systemUnderTest.ContactPhoneNumber.Should().BeNullOrEmpty();
        _systemUnderTest.ContactSubmittedDate.Should().Be(DateTime.MinValue);
        _systemUnderTest.ContactLastModifiedDate.Should().Be(DateTime.MinValue);

        _systemUnderTest.AuthSignatoryName.Should().BeNullOrEmpty();
        _systemUnderTest.AuthSignatoryPosition.Should().BeNullOrEmpty();
        _systemUnderTest.AuthSignatoryEmail.Should().BeNullOrEmpty();
        _systemUnderTest.AuthSignatorySubmittedDate.Should().Be(DateTime.MinValue);
        _systemUnderTest.AuthSignatoryLastModifiedDate.Should().Be(DateTime.MinValue);
    }

    [Test]
    public void OnGetChangeContactDetails_Redirect_Successfully()
    {
        var tradePartyId = Guid.NewGuid();
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeUpdateContactPath,
            new { id = tradePartyId});

        // Act
        var result = _systemUnderTest?.OnGetChangeContactDetails(tradePartyId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
    }

    [Test]
    public void OnGetChangeAuthRepresentativeDetails_Redirect_Successfully()
    {
        var tradePartyId = Guid.NewGuid();
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeUpdateAuthRepPath,
            new { id = tradePartyId });

        // Act
        var result = _systemUnderTest?.OnGetChangeAuthRepresentativeDetails(tradePartyId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
    }

}
