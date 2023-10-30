using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.CheckYourAnswers
{
    [TestFixture]
    public class CheckYourAnswersTests : PageModelTestsBase
    {
        private CheckYourAnswersModel? _systemUnderTest;
        protected Mock<ILogger<CheckYourAnswersModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();
        protected Mock<ITraderService> _mockTraderService = new();
        protected Mock<ICheckAnswersService> _mockCheckAnswersService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new CheckYourAnswersModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object, _mockCheckAnswersService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_EmptyRegistrationId_RedirectCorrectly()
        {
            // arrange

            var tradePartyId = Guid.Empty;
            var logisticsLocations = new List<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "England" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _systemUnderTest!.RegistrationID = tradePartyId;
            var expected = new RedirectToPageResult(
              Routes.Pages.Path.RegisteredBusinessCountryPath,
               new { id = _systemUnderTest!.RegistrationID });

            // act
            var result = await _systemUnderTest!.OnGetAsync(tradePartyId);

            // assert

            result.Should().BeOfType<RedirectToPageResult>();
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesNI()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var logisticsLocations = new List<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            // act
            await _systemUnderTest!.OnGetAsync(tradePartyId);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("NI");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Places of destination");
            _systemUnderTest.ContentText.Should().Be("destination");
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesGB()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var logisticsLocations = new List<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "England" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            // act
            await _systemUnderTest!.OnGetAsync(tradePartyId);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("GB");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Places of dispatch");
            _systemUnderTest.ContentText.Should().Be("dispatch");
        }

        [Test]
        public async Task OnGetRemoveEstablishment__NoneLeft_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var NI_GBFlag = "NI";
            var logisticsLocations = new List<LogisticsLocationDto> { };
            var logisticsLocation = new LogisticsLocationDto()
            {
                Id = Guid.NewGuid(),
            };
            var establishmentId = logisticsLocation.Id;
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id).Result).Returns(logisticsLocation);
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);

            // act
            var result = await _systemUnderTest!.OnGetRemoveEstablishment(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.RegistrationTaskListPath, new { id = tradePartyId, NI_GBFlag });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
        }

        [Test]
        public void OnGetChangeEstablishmentAddress_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            // act
            var result = _systemUnderTest!.OnGetChangeEstablishmentAddress(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = tradePartyId, establishmentId, NI_GBFlag });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
        }

        [Test]
        public void OnGetChangeEmail_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            // act
            var result = _systemUnderTest!.OnGetChangeEmail(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentContactEmailPath, new { id = tradePartyId, locationId = establishmentId, NI_GBFlag });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
        }

        [Test]
        public void OnPostAnswersComplete_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };
            var logisticLocations = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            var tradePartyId = Guid.NewGuid();

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>())).ReturnsAsync(logisticLocations);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            _mockCheckAnswersService.Setup(x => x.IsLogisticsLocationsDataPresent(tradeParty, logisticLocations)).Returns(true);

            // act
            var result = _systemUnderTest!.OnPostSubmitAsync(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTermsAndConditionsPath, new { id = tradePartyId });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result.Result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result.Result!).RouteValues);
        }

        [Test]
        public void OnPostAnswersComplete_ContactDetailsPresent_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };
            var logisticLocations = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            var tradePartyId = Guid.NewGuid();

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>())).ReturnsAsync(logisticLocations);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            _mockCheckAnswersService.Setup(x => x.IsLogisticsLocationsDataPresent(tradeParty, logisticLocations)).Returns(true);

            // act
            var result = _systemUnderTest!.OnPostSubmitAsync(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTermsAndConditionsPath, new { id = tradePartyId });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result.Result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result.Result!).RouteValues);
        }

        [Test]
        public void OnPostAnswersNotComplete_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };
            var tradePartyId = Guid.NewGuid();

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(false);

            // act
            var result = _systemUnderTest!.OnPostSubmitAsync(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTaskListPath, new { id = tradePartyId });
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result.Result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result.Result!).RouteValues);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}