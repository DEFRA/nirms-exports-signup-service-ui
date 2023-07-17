using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Confirmation
{
    [TestFixture]
    public class SignUpConfirmationTests : PageModelTestsBase
    {
        protected Mock<ITraderService> _mockTraderService = new();
        private SignUpConfirmationModel? _systemUnderTest;

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new SignUpConfirmationModel(_mockTraderService.Object);
        }

        [Test]
        public async Task OnGet_ReturnsId()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();

            //act
            await _systemUnderTest!.OnGet(tradePartyId);

            //assert
            _systemUnderTest.TraderId.Should().Be(tradePartyId);
        }

        [Test]
        public void OnGet_EmailPopulated_WhenValidTraderIdPassedIn()
        {
            //Arrange
            var tradePartyId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDTO 
            { 
                Id = tradePartyId,
                PartyName = "AJ Associates",
                Contact = new TradeContactDTO { Id = Guid.NewGuid(), Email = "test@test.com" }
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradePartyDto);

            //Act
            _systemUnderTest?.OnGet(tradePartyId);

            //Assert
            _systemUnderTest?.Email?.Should().Be("test@test.com");

        }
    }
}
