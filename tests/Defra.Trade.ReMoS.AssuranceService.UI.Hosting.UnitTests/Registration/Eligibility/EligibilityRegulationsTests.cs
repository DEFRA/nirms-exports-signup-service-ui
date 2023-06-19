using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Eligibility;

[TestFixture]
public class EligibilityRegulationsTests : PageModelTestsBase
{
    protected Mock<ILogger<EligibilityRegulationsModel>> _mockLogger = new();
    private EligibilityRegulationsModel? _systemUnderTest;

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new EligibilityRegulationsModel(_mockLogger.Object);
    }

    [Test]
    public async Task OnGet_NoSavedData()
    {
        //arrange
        var expected = Guid.Empty;

        //act
        await _systemUnderTest!.OnGetAsync(Guid.Empty);
        var result = _systemUnderTest.TraderId;

        //assert
        result.Should().Be(expected);

    }

    [Test]
    public async Task OnGet_GuidOnEntryIsKept()
    {
        //arrange
        var expected = Guid.NewGuid();

        //act
        await _systemUnderTest!.OnGetAsync(expected);
        var result = _systemUnderTest.TraderId;

        //assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task OnPost_GuidOnEntryIsKept()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.TraderId = expected;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var result = _systemUnderTest.TraderId;

        //assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task OnPostSubmit_GuidFalseError()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.Confirmed = false;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var validation = _systemUnderTest.ModelState.ErrorCount;

        // assert
        validation.Should().Be(1);
    }

    [Test]
    public async Task OnPostSubmit_GuidTrueNoError()
    {
        //arrange
        var expected = Guid.NewGuid();
        _systemUnderTest!.Confirmed = true;

        //act
        await _systemUnderTest!.OnPostSubmitAsync();
        var validation = _systemUnderTest.ModelState.ErrorCount;

        // assert
        validation.Should().Be(0);
    }

}
