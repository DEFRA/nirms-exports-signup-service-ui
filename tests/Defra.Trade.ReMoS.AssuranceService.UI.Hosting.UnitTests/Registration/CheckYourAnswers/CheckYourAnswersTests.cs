﻿using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.CheckYourAnswers;
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

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new CheckYourAnswersModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesNI()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var logisticsLocations = new List<LogisticsLocationDTO>();
            var tradeParty = new TradePartyDTO { Address = new TradeAddressDTO { TradeCountry = "NI" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);

            // act
            await _systemUnderTest!.OnGetAsync(tradePartyId);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("NI");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Points of destination");
            _systemUnderTest.ContentText.Should().Be("destination");
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesGB()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var logisticsLocations = new List<LogisticsLocationDTO>();
            var tradeParty = new TradePartyDTO { Address = new TradeAddressDTO { TradeCountry = "England" } };

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(tradePartyId).Result).Returns(tradeParty);

            // act
            await _systemUnderTest!.OnGetAsync(tradePartyId);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("GB");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.ContentHeading.Should().Be("Places of dispatch");
            _systemUnderTest.ContentText.Should().Be("dispatch");
        }

        [Test]
        public async Task OnGetRemoveEstablishment_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";
            var logisticsLocations = new List<LogisticsLocationDTO> { };

            _mockEstablishmentService.Setup(x => x.RemoveEstablishmentAsync(establishmentId).Result);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);

            // act
            var result = await _systemUnderTest!.OnGetRemoveEstablishment(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = tradePartyId, NI_GBFlag });
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
    }
}
