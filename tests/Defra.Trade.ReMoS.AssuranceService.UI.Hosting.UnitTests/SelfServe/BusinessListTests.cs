using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe;

[TestFixture]
public class BusinessListTests : PageModelTestsBase
{
    private BusinessListModel? _systemUnderTest;
    protected Mock<ILogger<BusinessListModel>> _mockLogger = new();
    private readonly Mock<ITraderService> _mockTraderService = new();
    private readonly Mock<IUserService> _mockUserService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new BusinessListModel(
            _mockLogger.Object,
            _mockTraderService.Object,
            _mockUserService.Object
            )
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
    }

    [Test]
    public async Task OnGet_ModelPropertiesSetToValidValues_IfDataPresent()
    {
        // Arrange
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);

        // Act
        await _systemUnderTest!.OnGetAsync();

        // Assert
        Assert.That(_systemUnderTest.Businesses, Is.EqualTo(userOrgs));
    }

    [Test]
    public void OnGetNavigateToBusinessDashboard_RedirectsToUpdatedTermsAndConditions()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.UpdatedTermsAndConditionsPath,
            new { id = tradePartyId });

        // Act
        var result = _systemUnderTest!.OnGetNavigateToBusinessDashboard(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_AddModelError_IfOrgNotFound()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeDashboardPath,
            new { id = tradePartyId });

        // Act
        var result = await _systemUnderTest!.OnGetNavigateToSignup(orgId);

        // Assert
        _systemUnderTest.ModelState.Should().NotBeNull();
        _systemUnderTest.ModelState.Count.Should().Be(1);
    }

    [Test]
    public async Task OnGetNavigateToSignUp_IfOrgNotEnrolled_And_UserIsAdmin_RedirectToExporterService()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446")));

        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServicePath,
            new { id = tradePartyId });

        // Act
        var result = await _systemUnderTest!.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_IfOrgNotEnrolled_And_UserIsStandard_RedirectTo_RegisterBusinessForExporterServiceNonAdmin()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));

        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServiceNonAdminPath,
            new { id = tradePartyId });

        // Act
        var result = await _systemUnderTest!.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_IfOrgNotEnrolled_And_UserRoleNotFound_AddModelError()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = false },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));

        // Act
        var result = await _systemUnderTest!.OnGetNavigateToSignup(orgId);

        // Assert
        _systemUnderTest.ModelState.Should().NotBeNull();
        _systemUnderTest.ModelState.Count.Should().Be(1);
    }

    [Test]
    public async Task OnGetRefreshBusinesses_ReturnRedirectToPageResult()
    {
        // Arrange
        var expected = new RedirectToPageResult("/Index");

        //mock the Http Context along with Service provider
        var mockHttpContext = new Mock<HttpContext>();
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult((object)null!));
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(_ => _.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
        mockHttpContext
            .Setup(x => x.RequestServices)
            .Returns(serviceProviderMock.Object);

        _systemUnderTest = new BusinessListModel(_mockLogger.Object, _mockTraderService.Object, _mockUserService.Object)
        {
            PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        // Act
        var result = await _systemUnderTest!.OnGetRefreshBusinesses();

        // Assert
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_When_SignupSTatus_Is_InProgressEligibilityRegulations_RedirectToRegulationsPage()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var tradePartyId = Guid.NewGuid();
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, TradePartySignupStatus.InProgressEligibilityRegulations));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = _systemUnderTest!.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_When_SignupSTatus_Is_InProgressEligibilityFboNumber_RedirectToFboNumberPage()
    {
        // Arrange
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, TradePartySignupStatus.InProgressEligibilityFboNumber));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessFboNumberPath,
            new { id = _systemUnderTest.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_When_SignupSTatus_Is_InProgressEligibilityCountry_RedirectToCountryPage()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, TradePartySignupStatus.InProgressEligibilityCountry));
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = _systemUnderTest.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignUp_When_SignupSTatus_Is_InProgress_RedirectToTaskListPage()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(_systemUnderTest!.User, It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, TradePartySignupStatus.InProgress));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = _systemUnderTest.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignup_When_SignupSTatus_Is_Complete_RedirectToAlreadyRegisteredPage()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, TradePartySignupStatus.Complete));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
            new { id = _systemUnderTest.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }

    [Test]
    public async Task OnGetNavigateToSignup_When_SignupStatus_Is_New_RedirectToRegulationsPage()
    {
        // Arrange
        var orgId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649");
        var userOrgs = new List<Organisation>()
        {
            new Organisation() { OrganisationId = Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649"), PracticeName = "local org 1", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("dc393799-e338-4998-8862-3c005dba351a"), PracticeName = "local org 2", Enrolled = true, UserRole = "Standard" },
            new Organisation() { OrganisationId = Guid.Parse("ffe013e4-79d7-4c3c-a96d-a17143525446"), PracticeName = "local org 3", Enrolled = false, UserRole = "Admin" },
        };
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(_systemUnderTest!.User))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
            .Returns(userOrgs.FirstOrDefault(o => o.OrganisationId == Guid.Parse("eb506178-ddd8-466d-abfd-99efc4e3a649")));
        _systemUnderTest!.TradePartyId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, TradePartySignupStatus.New));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = _systemUnderTest.TradePartyId });

        // Act
        var result = await _systemUnderTest.OnGetNavigateToSignup(orgId);

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
    }
}