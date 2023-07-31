using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessBusinessPickerTests
{
    private RegisteredBusinessBusinessPickerModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessBusinessPickerModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTraderService = new();
    protected Mock<IUserService> _mockUserService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessBusinessPickerModel(_mockLogger.Object, _mockTraderService.Object, _mockUserService.Object);
    }

    [Test]
    public async Task OnGetAsync_IfEmptyIdPassedIn_TraderIdShouldBeSetToEmpty()
    {
        //Arrange
        var id = Guid.Empty;

        //Act
        await _systemUnderTest!.OnGetAsync();

        //Assert
        _systemUnderTest.TraderId.Should().Be(Guid.Empty);
    }

    [Test]
    public async Task OnPostSubmitAsync_IfAnotherBusinessSelected_AddModelError()
    {
        // Arrange
        _systemUnderTest!.SelectedBusiness = "Another business";

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("UnregisteredBusiness").Should().BeTrue();
    }

    [Test]
    public async Task OnPostSubmitAsync_When_SignupStatus_Is_New_RedirectToCountryPage()
    {
        // Arrange
        var userOrgs = new Dictionary<Guid, string>();
        userOrgs.Add(Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"), "org1");
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync(((TradePartyDTO)null!, Core.Enums.TradePartySignupStatus.New));
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
        var userOrgs = new Dictionary<Guid, string>();
        userOrgs.Add(Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"), "org1");
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDTO { Id = Guid.NewGuid()}, Core.Enums.TradePartySignupStatus.Complete));
        _mockUserService
            .Setup(x => x.GetDefraOrgsForUser(It.IsAny<ClaimsPrincipal>()))
            .Returns(userOrgs);
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
        var userOrgs = new Dictionary<Guid, string>();
        userOrgs.Add(Guid.Parse("247d3fca-d874-45c8-b2ab-024b7bc8f701"), "org1");
        _systemUnderTest!.SelectedBusiness = "247d3fca-d874-45c8-b2ab-024b7bc8f701";
        _systemUnderTest.TraderId = Guid.NewGuid();
        _mockTraderService
            .Setup(x => x.GetDefraOrgBusinessSignupStatus(It.IsAny<Guid>()))
            .ReturnsAsync((new TradePartyDTO { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgress));
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
            .ReturnsAsync((new TradePartyDTO { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityCountry));
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
            .ReturnsAsync((new TradePartyDTO { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityFboNumber));
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
            .ReturnsAsync((new TradePartyDTO { Id = Guid.NewGuid() }, Core.Enums.TradePartySignupStatus.InProgressEligibilityRegulations));
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.RegisteredBusinessRegulationsPath,
            new { id = _systemUnderTest.TraderId });

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
    }
}
