﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class ConfirmEstablishmentDetailsTests : PageModelTestsBase
{
    private ConfirmEstablishmentDetailsModel? _systemUnderTest;
    protected Mock<ILogger<ConfirmEstablishmentDetailsModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new ConfirmEstablishmentDetailsModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_SetValidValues()
    {
        //Arrange
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com"
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
        _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));
        _mockEstablishmentService.Setup(x => x.IsEstablishmentDraft(It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        var result = await _systemUnderTest!.OnGetAsync(new Guid(), logisticsLocation.Id, It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
        _systemUnderTest.Email.Should().Be("test@test.com");
    }

    [Test]
    public void OnPostSubmit_ModelIsValid()
    {
        //Act
        _systemUnderTest!.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGetRemoveEstablishment_SubmitIsValid()
    {
        //Arrange
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = new Guid()
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
        _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));

        //Act
        await _systemUnderTest!.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGetRemoveEstablishment_GivenExistingLocations_SubmitIsValid()
    {
        //Arrange
        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = new Guid()
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
        _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));
        //Act
        await _systemUnderTest!.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGetRemoveEstablishment_GivenNoExistingLocations_Redirect()
    {
        //Arrange
        var list = new List<LogisticsLocationDto> { };
        var pagedList = new PagedList<LogisticsLocationDto>() { Items = list };
        var logisticsLocation = new LogisticsLocationDto()
        {
            Id = new Guid()
        };
        _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
        _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));
        _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid(), false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()).Result).Returns(pagedList);

        //Act
        await _systemUnderTest!.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public void OnGetChangeEstablishmentAddress_SubmitIsValid()
    {
        //Act
        _systemUnderTest!.OnGetChangeEstablishmentAddress(new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public void OnGetChangeEstablishmentAddress_GivenAddedManually_SubmitIsValid()
    {
        //Arrange
        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };

        //Act
        _systemUnderTest!.OnGetChangeEstablishmentAddress(new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public void OnGetChangeEstablishmentAddress_GivenNotAddedManually_Redirect()
    {
        //Arrange
        var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };

        //Act
        _systemUnderTest!.OnGetChangeEstablishmentAddress(new Guid(), new Guid(), It.IsAny<string>());
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public void OnGetChangeEmail_Redirect_Successfully()
    {
        var orgId = Guid.NewGuid();
        var establishmentId = Guid.NewGuid();
        string NI_GBFlag = "England";
        var expected = new RedirectToPageResult(
            Routes.Pages.Path.SelfServeEstablishmentContactEmailPath,
            new { id = orgId, locationId = establishmentId, NI_GBFlag });

        // Act
        var result = _systemUnderTest?.OnGetChangeEmail(orgId, establishmentId, NI_GBFlag);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectToPageResult>();
        Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
        Assert.That(((RedirectToPageResult)result!).RouteValues, Is.EqualTo(expected.RouteValues));
    }

    [Test]
    public async Task OnGet_HeadingSetToParameter_Successfully_ForNI()
    {
        //Arrange
        var expectedHeading = "Add a place of destination";
        var expectedContentText = "destination";
        _mockEstablishmentService.Setup(x => x.IsEstablishmentDraft(It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        _systemUnderTest.ContentText.Should().Be(expectedContentText);
    }

    [Test]
    public async Task OnGet_HeadingSetToParameter_Successfully_ForGB()
    {
        //Arrange
        var expectedHeading = "Add a place of dispatch";
        var expectedContentText = "dispatch";
        _mockEstablishmentService.Setup(x => x.IsEstablishmentDraft(It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "England");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        _systemUnderTest.ContentText.Should().Be(expectedContentText);
    }

    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), It.IsAny<Guid>(), It.IsAny<string>());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGetAsync_RedirectsToEstablishmentErrorPage()
    {
        //Arrange
        _mockEstablishmentService.Setup(x => x.IsEstablishmentDraft(It.IsAny<Guid>())).ReturnsAsync(false);

        //Act
        var result = await _systemUnderTest!.OnGetAsync(new Guid(), Guid.NewGuid(), It.IsAny<string>());

        // assert
        result.Should().NotBeNull();
        result.GetType().Should().Be(typeof(RedirectToPageResult));
        ((RedirectToPageResult)result).PageName.Should().Be(Routes.Pages.Path.EstablishmentErrorPath);
    }
}