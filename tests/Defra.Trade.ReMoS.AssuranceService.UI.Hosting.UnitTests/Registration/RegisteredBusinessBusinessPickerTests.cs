using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.ViewModels;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessBusinessPickerTests
{
    private RegisteredBusinessBusinessPickerModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessBusinessPickerModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<IUserService> _mockUserService = new();
    Mock<HttpContext> _httpContextMock = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessBusinessPickerModel(_mockLogger.Object, _mockTraderService.Object, _mockUserService.Object);
    }

    [Test]
    public void OnGetAsync_IfEmptyIdPassedIn_TraderIdShouldBeSetToEmpty()
    {
        //Arrange
        var id = Guid.Empty;

        //Act
        _systemUnderTest!.OnGet();

        //Assert
        _systemUnderTest.TraderId.Should().Be(Guid.Empty);
    }

    [Test]
    public void OnGetAsync_BuildSelectList_IfMoreThan7Businesses()
    {
        //Arrange
        _systemUnderTest!.TraderId = Guid.NewGuid();
        var userOrgs = new List<Organisation>
        {
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org1", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org2", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org3", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org4", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org5", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org6", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org7", Enrolled = false, UserRole = "Standard" },
            new Organisation {OrganisationId = Guid.NewGuid(), PracticeName = "org8", Enrolled = false, UserRole = "Standard" },
        };

        _mockUserService
        .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
        .Returns(userOrgs);


        //Act
        _systemUnderTest!.OnGet();

        //Assert
            // Includes choose business & Another business options
        _systemUnderTest.BusinessSelectList.Count.Should().Be(10);
    }

    [Test]
    public async Task OnPostSubmitAsync_IfAnotherBusinessSelected_RouteToErrorPage()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "Another business";
        _systemUnderTest!.TraderId = Guid.NewGuid();
        var expected = new RedirectToPageResult(
           Routes.Pages.Path.RegisteredBusinessPickerNoBusinessPickedPath,
           new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_IfChooseBusinessSelected_AddModelError()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "Choose business";

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("SelectedBusiness").Should().BeTrue();
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupStatus_Is_New_RedirectToCountryPage()
    {
        // Arrange
        var userOrgs = new List<Organisation>();
        userOrgs.Add(new Organisation { OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"), PracticeName = "org1", Enrolled = false, UserRole = "Standard"});
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDto)null!, Core.Enums.TradePartySignupStatus.New));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupSTatus_Is_Complete_RedirectToAlreadyRegisteredPage()
    {
        // Arrange
        var userOrg = new Organisation
        {
            OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),
            PracticeName = "org1",
            Enrolled = true,
            UserRole = "Standard"
        };
        var userOrgs = new List<Organisation>();
        userOrgs.Add(userOrg);
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.Complete));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        _mockUserService
            .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
            .Returns(userOrg);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessAlreadyRegisteredPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupSTatus_Is_InProgress_RedirectToTaskListPage()
    {
        // Arrange
        var userOrgs = new List<Organisation>();
        userOrgs.Add(new Organisation { OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"), PracticeName = "org1", Enrolled = false, UserRole = "Standard" });
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgress));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegistrationTaskListPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupSTatus_Is_InProgressEligibilityCountry_RedirectToCountryPage()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityCountry));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessCountryPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupSTatus_Is_InProgressEligibilityFboNumber_RedirectToFboNumberPage()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityFboNumber));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessFboNumberPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupSTatus_Is_InProgressEligibilityRegulations_RedirectToRegulationsPage()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDto { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityRegulations));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }

    [Test]
    public async Task OnPostSubmitAsync_WhenOrgNotEnrolledAndUserIsAdmin_ReturnRegisterBusinessForExporterServicePage()
    {
        // Arrange
        var userOrg = new Organisation
        {
            OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),
            PracticeName = "org1",
            Enrolled = false,
            UserRole = "Admin"
        };
        var userOrgs = new List<Organisation>();
        userOrgs.Add(userOrg);
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockUserService
           .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
           .Returns(userOrg);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServicePath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }
    [Test]
    public async Task OnPostSubmitAsync_WhenOrgNotEnrolledAndUserIsNotAdmin_ReturnRegisterBusinessForExporterServiceNonAdminPage()
    {
        // Arrange
        var userOrg = new Organisation
        {
            OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),
            PracticeName = "org1",
            Enrolled = false,
            UserRole = "Standard"
        };
        var userOrgs = new List<Organisation>();
        userOrgs.Add(userOrg);
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockUserService
           .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
           .Returns(userOrg);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServiceNonAdminPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }
    [Test]
    public async Task OnPostSubmitAsync_WhenOrgNotEnrolledAndUserNotFoundInToken_ReturnModelError()
    {
        // Arrange
        var userOrg = new Organisation
        {
            OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),
            PracticeName = "org1",
            Enrolled = false,
        };
        var userOrgs = new List<Organisation>();
        userOrgs.Add(userOrg);
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockUserService
           .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
           .Returns(userOrg);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServicePath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("SelectedBusiness", "User role not found").Should().BeTrue();
    }

    [Test]
    public async Task OnPostSubmitAsync_WhenOrgNotEnrolledAndUserRoleNotDefined_ReturnModelError()
    {
        // Arrange
        var userOrg = new Organisation
        {
            OrganisationId = Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"),
            PracticeName = "org1",
            Enrolled = false,
            UserRole = "New role"
        };
        var userOrgs = new List<Organisation>();
        userOrgs.Add(userOrg);
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockUserService
           .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
           .Returns(userOrg);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServicePath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("SelectedBusiness", "User role not found").Should().BeTrue();
        
    }

    [Test]
    public async Task OnPostSubmitAsync_WhenNoOrgDetailsRetrievedFromToken_ReturnModelError()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockUserService
           .Setup(x => x.GetOrgDetailsById(It.IsAny<ClaimsPrincipal>(), It.IsAny<Guid>()))
           .Returns((Organisation)null!);
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisterBusinessForExporterServicePath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("SelectedBusiness").Should().BeTrue();
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

        _systemUnderTest = new RegisteredBusinessBusinessPickerModel(_mockLogger.Object, _mockTraderService.Object, _mockUserService.Object)
        {
            PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = mockHttpContext.Object
            }
        };

        // Act
        var result = await _systemUnderTest!.OnGetRefreshBusinesses();

        // Assert
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }
}
