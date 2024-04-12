using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
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
    private readonly Mock<IFeatureManager> _mockFeatureManager = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new SelfServeDashboardModel(
            _mockLogger.Object, 
            _mockTraderService.Object, 
            _mockEstablishmentService.Object, 
            _checkAnswersService,
            _mockFeatureManager.Object)
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf") });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_ModelPropertiesSetToValidValues_IfDataPresentInApi()
    {
        //Arrange
        Guid guid = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
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
        var logisticsLocations = new List<LogisticsLocationDto>()
        { new LogisticsLocationDto()
            {
                Id = Guid.NewGuid(),
                NI_GBFlag = "GB",
                TradePartyId = guid,
                TradeAddressId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cc"),
                Address = new TradeAddressDto()
                {
                    Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cc")
                }
            }
        };

        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyDto!);
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>(), true).Result).Returns(logisticsLocations);

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

        //Assert

        _systemUnderTest.TradePartyId.Should().Be(guid);
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

        _systemUnderTest.LogisticsLocations!.FirstOrDefault()!.TradePartyId.Should().Be(guid);
    }

    [Test]
    public async Task OnGet_ModelProperties_SetToNull_IfNoSavedData()
    {
        //Arrange
        Guid guid = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
        TradePartyDto tradePartyDto = null!;
        _mockTraderService
            .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(tradePartyDto)!);

        //Act
        await _systemUnderTest!.OnGetAsync(guid);

        //Assert

        _systemUnderTest.TradePartyId.Should().Be(guid);
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
        var orgId = Guid.NewGuid();
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeUpdateContactPath,
            new { id = orgId});

        // Act
        var result = _systemUnderTest?.OnGetChangeContactDetails(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
    }

    [Test]
    public void OnGetChangeAuthRepresentativeDetails_Redirect_Successfully()
    {
        var orgId = Guid.NewGuid();
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeUpdateAuthRepPath,
            new { id = orgId });

        // Act
        var result = _systemUnderTest?.OnGetChangeAuthRepresentativeDetails(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [TestCase(LogisticsLocationApprovalStatus.Approved, "/SelfServe/Establishments/ViewEstablishment")]
    [TestCase(LogisticsLocationApprovalStatus.Suspended, "/SelfServe/Establishments/ViewEstablishment")]
    [TestCase(LogisticsLocationApprovalStatus.Removed, "/SelfServe/Establishments/ViewEstablishment")]
    [TestCase(LogisticsLocationApprovalStatus.Draft, "/SelfServe/Establishments/ConfirmEstablishmentDetails")]
    public async Task OnGetViewEstablishment_Redirects_Successfully(LogisticsLocationApprovalStatus status, string path)
    {
        // arrage
        var orgId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var NI_GBFlag = "GB";
        var expected = new RedirectToPageResult(path, new { id = orgId, locationId, NI_GBFlag });
        _mockEstablishmentService.Setup(x => x.IsEstablishmentDraft(It.IsAny<Guid>())).ReturnsAsync(true);

        // act
        var result = await _systemUnderTest!.OnGetViewEstablishment(orgId, locationId, NI_GBFlag, status);

        // assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
    }


    [TestCase(LogisticsLocationApprovalStatus.None)]
    [TestCase(LogisticsLocationApprovalStatus.Rejected)]
    public async Task OnGetViewEstablishment_Returns_Successfully(LogisticsLocationApprovalStatus status)
    {
        // arrage
        var orgId = Guid.NewGuid();
        var locationId = Guid.NewGuid();
        var NI_GBFlag = "GB";

        // act
        var result = await _systemUnderTest!.OnGetViewEstablishment(orgId, locationId, NI_GBFlag, status);

        // assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PageResult>();
    }

}
