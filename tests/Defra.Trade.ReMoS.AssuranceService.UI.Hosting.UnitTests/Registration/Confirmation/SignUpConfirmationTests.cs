using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.Confirmation;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
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
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_ReturnsId()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = tradePartyId,
                RemosBusinessSchemeNumber = "RMS-GB-000002"
            };
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradePartyDto);

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
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            _systemUnderTest?.OnGet(tradePartyId);

            //Assert
            _systemUnderTest?.Email?.Should().Be("test@test.com");
        }

        [Test]
        public void OnGetAnswersNotComplete_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { RemosBusinessSchemeNumber = "RMS-NI-000002", Address = new TradeAddressDto { TradeCountry = "NI" } };
            var tradePartyId = Guid.NewGuid();

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).
                ReturnsAsync(true);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(false);

            // act
            var result = _systemUnderTest!.OnGet(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTermsAndConditionsPath, new { id = tradePartyId });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result.Result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result.Result!).RouteValues);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGet(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [Test]
        [TestCase("RMS-GB-000002", "GB")]
        [TestCase("RMS-NI-000002", "NI")]
        [TestCase("RMS-Ngg-000002", "")]
        [TestCase("Invalid", "")]
        public void RetrieveGB_NIFLAG_TESTS(string remosNumber, string expectedResult)
        {
            var actualResult = _systemUnderTest!.RetrieveGB_NIFLAG(remosNumber);
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}