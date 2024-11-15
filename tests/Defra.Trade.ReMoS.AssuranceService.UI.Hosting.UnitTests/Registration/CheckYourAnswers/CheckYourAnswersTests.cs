﻿using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;

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
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf") });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        }

        [Test]
        public async Task OnGet_EmptyRegistrationId_RedirectCorrectly()
        {
            // arrange

            var tradePartyId = Guid.Empty;
            var logisticsLocations = new Core.Helpers.PagedList<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "England" } };

            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()).Result)
                .Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Empty });
            _systemUnderTest!.TradePartyId = tradePartyId;
            var expected = new RedirectToPageResult(
              Routes.Pages.Path.RegisteredBusinessCountryPath,
               new { id = _systemUnderTest!.TradePartyId });

            // act
            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            // assert

            result.Should().BeOfType<RedirectToPageResult>();
            Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesNI()
        {
            // arrange
            var tradePartyId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
            var logisticsLocations = new Core.Helpers.PagedList<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            // act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("NI");
            _systemUnderTest.TradePartyId.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Places of destination");
            _systemUnderTest.ContentText.Should().Be("destination");
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesGB()
        {
            // arrange
            var tradePartyId = Guid.Parse("73858931-5bc4-40ce-a735-fd8e82e145cf");
            var logisticsLocations = new Core.Helpers.PagedList<LogisticsLocationDto>();
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "England" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);

            // act
            await _systemUnderTest!.OnGetAsync(Guid.NewGuid());

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("GB");
            _systemUnderTest.TradePartyId.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Places of dispatch");
            _systemUnderTest.ContentText.Should().Be("dispatch");
        }

        [Test]
        public async Task OnGetRemoveEstablishment__NoneLeft_Redirect_Successfully()
        {
            // arrange
            var orgId = Guid.NewGuid();
            var tradePartyId = Guid.NewGuid();
            var NI_GBFlag = "NI";
            var logisticsLocations = new Core.Helpers.PagedList<LogisticsLocationDto> { };
            var logisticsLocation = new LogisticsLocationDto()
            {
                Id = Guid.NewGuid(),
            };
            var establishmentId = logisticsLocation.Id;
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id).Result).Returns(logisticsLocation);
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()).Result).Returns(logisticsLocations);

            // act
            var result = await _systemUnderTest!.OnGetRemoveEstablishment(orgId, tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.RegistrationTaskListPath, new { id = orgId, NI_GBFlag });
            Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result!).RouteValues, Is.EqualTo(expected.RouteValues));
        }

        [Test]
        public void OnGetChangeEstablishmentAddress_Redirect_Successfully()
        {
            // arrange
            var orgId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            // act
            var result = _systemUnderTest!.OnGetChangeEstablishmentAddress(orgId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = orgId, establishmentId, NI_GBFlag });
            Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result!).RouteValues, Is.EqualTo(expected.RouteValues));
        }

        [Test]
        public void OnGetChangeEmail_Redirect_Successfully()
        {
            // arrange
            var orgId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            // act
            var result = _systemUnderTest!.OnGetChangeEmail(orgId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentContactEmailPath, new { id = orgId, locationId = establishmentId, NI_GBFlag });
            Assert.That(((RedirectToPageResult)result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result!).RouteValues, Is.EqualTo(expected.RouteValues));
        }

        [Test]
        public void OnPostAnswersComplete_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };
            var logisticLocations = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            var pagedLocation = new PagedList<LogisticsLocationDto>() { Items = logisticLocations };
            var tradePartyId = Guid.NewGuid();

            _systemUnderTest!.TradePartyId = tradePartyId;
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedLocation);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            _mockCheckAnswersService.Setup(x => x.IsLogisticsLocationsDataPresent(tradeParty, logisticLocations)).Returns(true);

            // act
            var result = _systemUnderTest!.OnPostSubmitAsync(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTermsAndConditionsPath, new { id = tradePartyId });
            Assert.That(((RedirectToPageResult)result.Result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result.Result!).RouteValues, Is.EqualTo(expected.RouteValues));
        }

        [Test]
        public void OnPostAnswersComplete_ContactDetailsPresent_Redirect_Successfully()
        {
            // arrange
            var tradeParty = new TradePartyDto { Address = new TradeAddressDto { TradeCountry = "NI" } };
            var logisticLocations = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            var pagedLocation = new PagedList<LogisticsLocationDto>() { Items = logisticLocations };
            var tradePartyId = Guid.NewGuid();

            _systemUnderTest!.TradePartyId = tradePartyId;
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedLocation);
            _mockCheckAnswersService.Setup(x => x.ReadyForCheckAnswers(tradeParty)).Returns(true);
            _mockCheckAnswersService.Setup(x => x.IsLogisticsLocationsDataPresent(tradeParty, logisticLocations)).Returns(true);

            // act
            var result = _systemUnderTest!.OnPostSubmitAsync(tradePartyId);

            // assert
            var expected =
                new RedirectToPageResult(Routes.Pages.Path.RegistrationTermsAndConditionsPath, new { id = tradePartyId });
            Assert.That(((RedirectToPageResult)result.Result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result.Result!).RouteValues, Is.EqualTo(expected.RouteValues));
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
            Assert.That(((RedirectToPageResult)result.Result!).PageName, Is.EqualTo(expected.PageName));
            Assert.That(((RedirectToPageResult)result.Result!).RouteValues, Is.EqualTo(expected.RouteValues));
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}