using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
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
public class SelectedBusinessTests
{
    private SelectedBusinessModel? _systemUnderTest;
    protected Mock<ILogger<SelectedBusinessModel>> _mockLogger = new();
    protected Mock<ITraderService> _mockTradeService = new();

    [SetUp]
    public void TestCaseSetup()
    {
        _systemUnderTest = new SelectedBusinessModel(_mockLogger.Object, _mockTradeService.Object);
    }

    [Test]
    public async Task OnGet_TraderId_ShouldBeSetToId()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        await _systemUnderTest!.OnGetAsync(id);

        //Assert
        _systemUnderTest.TradePartyId.Should().Be(id);
    }

    [Test]
    public async Task OnGet_Set_SelectedBusiness_To_BusinessNameFromApi()
    {
        //ARrange
        var tradePartyDto = new TradePartyDTO { Id = Guid.NewGuid(), PracticeName = "Test name" };
        _mockTradeService
            .Setup(action => action.GetTradePartyByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tradePartyDto);

        //Act
        await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>());

        //Assert
        _systemUnderTest.SelectedBusinessName.Should().Be(tradePartyDto.PracticeName);
    }
}
