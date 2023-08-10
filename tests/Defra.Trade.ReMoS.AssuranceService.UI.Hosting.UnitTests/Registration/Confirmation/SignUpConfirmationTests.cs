using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Configuration;
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
        protected Mock<ICheckAnswersService> _mockCheckAnswersService = new();
        private SignUpConfirmationModel? _systemUnderTest;
        private Mock<IConfiguration> _mockConfiguration = new();

        [SetUp]
        public void TestCaseSetup()
        {
            var _mockConfigSection = new Mock<IConfigurationSection>();
            _mockConfigSection.Setup(x => x.Value).Returns("testurl");
            _mockConfiguration.Setup(x => x.GetSection("ExternalLinks:StartNowPage")).Returns(_mockConfigSection.Object);
            _systemUnderTest = new SignUpConfirmationModel(_mockTraderService.Object, _mockCheckAnswersService.Object, _mockConfiguration.Object);
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
            _systemUnderTest.StartNowPage.Should().Be("testurl");
        }

        [Test]
        public void OnGet_EmailPopulated_WhenValidTraderIdPassedIn()
        {
            //Arrange
            var tradePartyId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = tradePartyId,
                PartyName = "AJ Associates",
                Contact = new TradeContactDto { Id = Guid.NewGuid(), Email = "test@test.com" }
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradePartyDto);

            //Act
            _systemUnderTest?.OnGet(tradePartyId);

            //Assert
            _systemUnderTest?.Email?.Should().Be("test@test.com");
        }
    }
}