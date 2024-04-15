using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Errors;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Errors;

[TestFixture]
public class EstablishmentErrorTests : PageModelTestsBase
{
    private EstablishmentErrorModel? _systemUnderTest;
    protected Mock<ILogger<EstablishmentErrorModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EstablishmentErrorModel(_mockLogger.Object);
    }

    [Test]
    public void OnGet_ModelPupulated()
    {
        // arrange
        var id = Guid.NewGuid();

        // act
        _systemUnderTest!.OnGet(id);

        // assert
        _systemUnderTest.OrgId.Should().Be(id);
    }
}
