using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
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
            .Setup(x => x.GetRelationshipBetweenPartyAndEstablishment(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new Core.DTOs.LogisticsLocationBusinessRelationshipDTO());

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
        var expectedHeading = "Add a point of destination (optional)";
        var expectedContentText = "Add all establishments in Northern Ireland where your goods go after the port of entry. For example, a hub or store.";

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "NI");

        //Assert
        _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        _systemUnderTest.ContentText.Should().Be(expectedContentText);
    }
}
