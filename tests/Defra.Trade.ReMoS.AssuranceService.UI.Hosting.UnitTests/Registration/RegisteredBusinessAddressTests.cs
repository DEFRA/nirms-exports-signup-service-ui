using Castle.Core.Logging;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessAddressTests : PageModelTestsBase
{
    protected Mock<ILogger<RegisteredBusinessAddressModel>> _mockLogger = new ();
    private RegisteredBusinessAddressModel? _systemUnderTest;

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessAddressModel(_mockLogger.Object);
    }

    [Test]
    public async Task OnGet_NoAddressPresentIfNoSavedData()
    {
        // arrange
        //TODO: add setup for returning value when api reference

        // act
        await _systemUnderTest.OnGetAsync();

        // assert
        _systemUnderTest.LineOne.Should().Be("");
        _systemUnderTest.LineTwo.Should().Be("");
        _systemUnderTest.CityName.Should().Be("");
        _systemUnderTest.PostCode.Should().Be("");
    }

    [Test]
    public async Task OnPostSubmit_SubmitValidInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "Line 1 - '";
        _systemUnderTest.LineTwo = "Line 2 - '";
        _systemUnderTest.CityName = "City - '";
        _systemUnderTest.PostCode = "P0S1 C0DE";

        // act 
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(0);
    }

    public async Task OnPostSubmit_SubmitInvalidCharacterInformation()
    {
        // arrange 
        _systemUnderTest.LineOne = "*";
        _systemUnderTest.LineTwo = "*";
        _systemUnderTest.CityName = "*";
        _systemUnderTest.PostCode = "*";

        // act 
        await _systemUnderTest.OnPostSubmitAsync();
        var validation = ValidateModel(_systemUnderTest);

        // assert
        validation.Count.Should().Be(4);
    }
}
