using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class AdditionalEstablishmentAddressTests : PageModelTestsBase
    {
        private AdditionalEstablishmentAddressModel? _systemUnderTest;
        protected Mock<ILogger<AdditionalEstablishmentAddressModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new AdditionalEstablishmentAddressModel(_mockLogger.Object, _mockEstablishmentService.Object);
        }

        [Test]
        public async Task OnGet_RadioNotSelected()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>());

            //Assert
            _systemUnderTest.AdditionalAddress.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidRadio()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "";
            var expectedResult = "Select yes if you want to add another place of dispatch";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidRadio_ModelStateIsInvalid()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "";
            var expectedResult = "Select yes if you want to add another place of dispatch";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_SubmitIsValid()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_GivenExistingLocations_SubmitIsValid()
        {
            //Arrange

            var list = new List<LogisticsLocationDTO> { new LogisticsLocationDTO() };
            _systemUnderTest!.AdditionalAddress = "yes";
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);

            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_GivenNoExistingLocations_Redirect()
        {
            //Arrange
            var list = new List<LogisticsLocationDTO> { };
            _systemUnderTest!.AdditionalAddress = "yes";
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);

            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }


        [Test]
        public async Task OnGetChangeEstablishmentAddress_SubmitIsValid()
        {
            //Arrange
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetChangeEstablishmentAddress_GivenAddedManually_SubmitIsValid()
        {
            //Arrange
            var list = new List<LogisticsLocationDTO> { new LogisticsLocationDTO() };
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetChangeEstablishmentAddress_GivenNotAddedManually_Redirect()
        {
            //Arrange
            var list = new List<LogisticsLocationDTO> { new LogisticsLocationDTO() };
            _systemUnderTest!.AdditionalAddress = "yes";

            //Act
            await _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public void OnGetChangeEmail_Redirect_Successfully()
        {
            var tradePartyId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            string NI_GBFlag = "GB";
            var expected = new RedirectToPageResult(
                Routes.Pages.Path.EstablishmentContactEmailPath, 
                new { id = tradePartyId, locationId = establishmentId, NI_GBFlag = string.Empty });

            // Act
            var result = _systemUnderTest?.OnGetChangeEmail(tradePartyId, establishmentId, string.Empty);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToPageResult>();
            Assert.AreEqual(expected.PageName, ((RedirectToPageResult)result!).PageName);
            Assert.AreEqual(expected.RouteValues, ((RedirectToPageResult)result!).RouteValues);
        }

        [Test]
        public async Task OnGet_HeadingSetToParameter_Successfully()
        {
            //Arrange
            var expectedHeading = "Places of destination (optional)";
            var expectedContentText = "destination";

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
            _systemUnderTest.ContentText.Should().Be(expectedContentText);
        }

    }
}
