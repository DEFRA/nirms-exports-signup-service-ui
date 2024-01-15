using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessAlreadyRegisteredTests : PageModelTestsBase
{
    private RegisteredBusinessAlreadyRegisteredModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessAlreadyRegisteredModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessAlreadyRegisteredModel(_mockLogger.Object);
    }

    [Test]
    public void OnGet_TradePartyId_ShouldBeSetToId()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        _systemUnderTest!.OnGetAsync(id);

        //Assert
        _systemUnderTest.OrgId.Should().Be(id);
    }
}
