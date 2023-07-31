using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments;

[TestFixture]
public class ContactEmailTests : PageModelTestsBase
{
    private ContactEmailModel? _systemUnderTest;
    protected Mock<ILogger<ContactEmailModel>> _mockLogger = new();
    protected Mock<IEstablishmentService> _mockEstablishmentService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new ContactEmailModel(_mockLogger.Object, _mockEstablishmentService.Object);
    }

    [Test]
    public async Task OnGet_NoRelationDtoPopulated_IfNoSavedData()
    {
        //Arrange
        _mockEstablishmentService
            .Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Core.DTOs.LogisticsLocationDTO());

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
        var expectedContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        _systemUnderTest.ContentText.Should().Be(expectedContentText);
    }

    [Test]
    public void OnGetChangeEstablishmentAddress_Returns_RedirectResult()
    {
        //Arrange
        var tradePartyId = new Guid();
        var establishmentId = new Guid();
        string NI_GBFlag = "GB";

        //Act
        var result = _systemUnderTest?.OnGetChangeEstablishmentAddress(tradePartyId, establishmentId, NI_GBFlag);

        //Assert
        result?.GetType().Should().Be(typeof(RedirectToPageResult));
        (result as RedirectToPageResult)?.PageName?.Equals(Routes.Pages.Path.EstablishmentNameAndAddressPath);
    }
}
