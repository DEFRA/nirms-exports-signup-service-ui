using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.SelfServe.Establishments;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.SelfServe.Establishments;

[TestFixture]
public class EstablishmentRemovedTests : PageModelTestsBase
{
    private EstablishmentRemovedModel? _systemUnderTest;

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EstablishmentRemovedModel()
        {
            PageContext = PageModelMockingUtils.MockPageContext()
        };
    }

    [TestCase("Dep warehouse", "GB")]
    [TestCase("Dest warehouse", "NI")]
    public void OnGet_ModelPropertiesSet_WhenValuesGiven(string locName, string NI_GB_Flag)
    {
        // arrange
        var id = Guid.NewGuid();

        // act
        var result = _systemUnderTest?.OnGet(id, locName, NI_GB_Flag);

        // assert
        result.Should().NotBeNull();
        _systemUnderTest?.OrgId.Should().Be(id);
        _systemUnderTest?.EstablishmentName.Should().Be(locName);
        _systemUnderTest?.NI_GBFlag.Should().Be(NI_GB_Flag);
    }
}
