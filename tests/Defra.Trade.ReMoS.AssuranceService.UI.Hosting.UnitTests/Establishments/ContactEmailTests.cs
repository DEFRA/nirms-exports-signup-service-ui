﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments;

[TestFixture]
public class ContactEmailTests : PageModelTestsBase
{
    private ContactEmailModel? _systemUnderTest;
    protected Mock<ILogger<ContactEmailModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();
    protected Mock<ITraderService> _mockTraderService = new();    

    [SetUp]
    public void TestCaseSetup()
    {        
        _systemUnderTest = new ContactEmailModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
        _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.NewGuid() });
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
    }

    [Test]
    public async Task OnGet_NoRelationDtoPopulated_IfNoSavedData()
    {
        //Arrange
        _mockEstablishmentService
            .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new LogisticsLocationDto());

        //Act
        await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid());

        //Assert
        _systemUnderTest.Email.Should().Be(null);
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidEmail()
    {
        //Arrange
        _systemUnderTest!.Email = "test@test.com";

        //Act
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        //Assert
        validation.Count.Should().Be(0);
    }

    [Test]
    public async Task OnGet_HeadingSetToParameter_Successfully()
    {
        //Arrange
        var expectedHeading = "Add a place of destination";

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
    }

    [Test]
    public void OnGetChangeEstablishmentAddress_Returns_RedirectResult()
    {
        //Arrange
        var orgId = new Guid();
        var establishmentId = new Guid();
        string NI_GBFlag = "GB";

        //Act
        var result = _systemUnderTest?.OnGetChangeEstablishmentAddress(orgId, establishmentId, NI_GBFlag);

        //Assert
        result?.GetType().Should().Be(typeof(RedirectToPageResult));
        (result as RedirectToPageResult)?.PageName?.Equals(Routes.Pages.Path.EstablishmentNameAndAddressPath);
    }


    [Test]
    public async Task OnGetAsync_InvalidOrgId()
    {
        _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
    }

    [Test]
    public async Task OnGetAsync_RedirectRegisteredBusiness()
    {
        _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

        var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid());
        var redirectResult = result as RedirectToPageResult;

        redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
    }

    [Test]
    public async Task OnPostSubmit_IfInputNotValid_ReloadPage()
    {
        //Arrange
        _systemUnderTest!.Email = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com";

        //Act
        var result = await _systemUnderTest.OnPostSubmitAsync();
        //var validation = ValidateModel(_systemUnderTest);

        //Assert
        result?.GetType().Should().Be(typeof(RedirectToPageResult));
        (result as RedirectToPageResult)?.PageName?.Equals(Routes.Pages.Path.EstablishmentContactEmailPath);
    }
}
