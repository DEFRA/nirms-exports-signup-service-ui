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

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new CheckYourAnswersModel(_mockLogger.Object, _mockEstablishmentService.Object);
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesNI()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var country = "NI";
            var logisticsLocations = new List<LogisticsLocationDetailsDTO>();

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);
        
            // act
            await _systemUnderTest.OnGetAsync(tradePartyId, country);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("NI");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.Country.Should().Be(country);
            _systemUnderTest.ContentHeading.Should().Be("Points of destination");
            _systemUnderTest.ContentText.Should().Be("destination");
        }

        [Test]
        public async Task OnGet_PopulateModelPropertiesGB()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var country = "England";
            var logisticsLocations = new List<LogisticsLocationDetailsDTO>();

            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);

            // act
            await _systemUnderTest.OnGetAsync(tradePartyId, country);

            // assert
            _systemUnderTest.NI_GBFlag.Should().Be("GB");
            _systemUnderTest.RegistrationID.Should().Be(tradePartyId);
            _systemUnderTest.Country.Should().Be(country);
            _systemUnderTest.ContentHeading.Should().Be("Points of departure");
            _systemUnderTest.ContentText.Should().Be("departure");
        }

        [Test]
        public async Task OnGetRemoveEstablishment_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";
            var logisticsLocations = new List<LogisticsLocationDetailsDTO> { };

            _mockEstablishmentService.Setup(x => x.RemoveEstablishmentFromPartyAsync(tradePartyId, establishmentId).Result);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(tradePartyId).Result).Returns(logisticsLocations);

            // act
            var result = await _systemUnderTest.OnGetRemoveEstablishment(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var exptected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = tradePartyId, NI_GBFlag });
            ((RedirectToPageResult)result!).PageName.Should().Be(exptected.PageName);
            ((RedirectToPageResult)result!).RouteValues["id"].Should().Be(tradePartyId);
            ((RedirectToPageResult)result!).RouteValues["NI_GBFlag"].Should().Be(NI_GBFlag);
        }

        [Test]
        public async Task OnGetChangeEstablishmentAddress_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            _mockEstablishmentService.Setup(x => x.IsFirstTradePartyForEstablishment(tradePartyId, establishmentId).Result).Returns(true);

            // act
            var result = await _systemUnderTest.OnGetChangeEstablishmentAddress(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentNameAndAddressPath, new { id = tradePartyId, NI_GBFlag });
            ((RedirectToPageResult)result!).PageName.Should().Be(expected.PageName);
            ((RedirectToPageResult)result!).RouteValues["id"].Should().Be(tradePartyId);
            ((RedirectToPageResult)result!).RouteValues["NI_GBFlag"].Should().Be(NI_GBFlag);
        }

        [Test]
        public async Task OnGetChangeEmail_Redirect_Successfully()
        {
            // arrange
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            var NI_GBFlag = "NI";

            // act
            var result = _systemUnderTest.OnGetChangeEmail(tradePartyId, establishmentId, NI_GBFlag);

            // assert
            var expected = new RedirectToPageResult(Routes.Pages.Path.EstablishmentContactEmailPath, new { id = tradePartyId, NI_GBFlag });
            ((RedirectToPageResult)result!).PageName.Should().Be(expected.PageName);
            ((RedirectToPageResult)result!).RouteValues["id"].Should().Be(tradePartyId);
            ((RedirectToPageResult)result!).RouteValues["NI_GBFlag"].Should().Be(NI_GBFlag);
        }
    }
}
