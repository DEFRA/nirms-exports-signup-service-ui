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
        protected Mock<ILogger<SignUpConfirmationModel>> _mockLogger = new();
        private SignUpConfirmationModel? _systemUnderTest;

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new SignUpConfirmationModel();
        }

        [Test]
        public void OnGet_ReturnsId()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();

            //act
            _systemUnderTest!.OnGet(tradePartyId);

            //assert
            _systemUnderTest.TraderId.Should().Be(tradePartyId);
        }
    }
}
