using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration;

[TestFixture]
public class RegisteredBusinessBusinessPickerTests
{
    private RegisteredBusinessBusinessPickerModel? _systemUnderTest;
    protected Mock<ILogger<RegisteredBusinessBusinessPickerModel>> _mockLogger = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new RegisteredBusinessBusinessPickerModel(_mockLogger.Object);
    }

    [Test]
    public async Task OnGetAsync_IfEmptyIdPassedIn_TraderIdShouldBeSetToEmpty()
    {
        //Arrange
        var id = Guid.Empty;

        //Act
        await _systemUnderTest!.OnGetAsync(id);

        //Assert
        _systemUnderTest.TraderId.Should().Be(Guid.Empty);
    }

    [Test]
    public async Task OnPostSubmitAsync_IfAnotherBusinessSelected_AddModelError()
    {
        // Arrange
        _systemUnderTest!.Business = "Another business";

        // Act
        var result = await _systemUnderTest.OnPostSubmitAsync();

        // Assert
        _systemUnderTest.ModelState.HasError("UnregisteredBusiness").Should().BeTrue();
    }
}
