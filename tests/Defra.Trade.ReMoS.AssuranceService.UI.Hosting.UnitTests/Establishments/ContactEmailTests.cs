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
    protected Mock<ITraderService> _mockTraderService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new ContactEmailModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
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
        _systemUnderTest.Email.Should().Be("");
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
}
