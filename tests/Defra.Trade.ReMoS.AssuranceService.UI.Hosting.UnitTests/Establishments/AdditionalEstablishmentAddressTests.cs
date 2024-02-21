using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Constants;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class AdditionalEstablishmentAddressTests : PageModelTestsBase
    {
        private AdditionalEstablishmentAddressModel? _systemUnderTest;
        protected Mock<ILogger<AdditionalEstablishmentAddressModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();
        protected Mock<ITraderService> _mockTraderService = new();
        protected Mock<ICheckAnswersService> _mockCheckAnswersService = new();       

        [SetUp]
        public void TestCaseSetup()
        {                      
            _systemUnderTest = new AdditionalEstablishmentAddressModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object, _mockCheckAnswersService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto(){ Id = Guid.NewGuid() });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        }

        [Test]
        public async Task OnGet_RadioNotSelected()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>());

            //Assert
            _systemUnderTest.AddAddressesComplete.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio()
        {
            //Arrange
            _systemUnderTest!.AddAddressesComplete = "yes";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSave_SubmitValidRadio()
        {
            //Arrange
            _systemUnderTest!.AddAddressesComplete = "yes";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_AnsweredNo()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "no";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_ContactDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "no";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_BusinessDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "no";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_AuthorisedSignatoryDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "no";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);

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
            _systemUnderTest!.AddAddressesComplete = "";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();

            //Assert
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.HasError("AddAddressesComplete").Should().Be(true);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_MissingBusinessDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "yes";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_MissingAuthorisedRepresentativeDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "yes";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);


            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidRadio_Redirected_MissingContactDetails()
        {
            //Arrange
            var tradeParty = new TradePartyDto();
            _systemUnderTest!.AddAddressesComplete = "yes";

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(new Guid()).Result).Returns(tradeParty);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetBusinessDetailsProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetAuthorisedSignatoryProgress(tradeParty)).Returns(TaskListStatus.COMPLETE);
            _mockCheckAnswersService.Setup(x => x.GetContactDetailsProgress(tradeParty)).Returns(TaskListStatus.NOTSTART);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSave_SubmitInValidRadio()
        {
            //Arrange
            _systemUnderTest!.AddAddressesComplete = "";

            //Act
            await _systemUnderTest.OnPostSaveAsync();

            //Assert
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.HasError("AddAddressesComplete").Should().Be(true);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_SubmitIsValid()
        {
            //Arrange
            _systemUnderTest!.AddAddressesComplete = "yes";
            var logisticsLocation = new LogisticsLocationDto()
            {
                Id = new Guid()
            };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));

            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_GivenExistingLocations_SubmitIsValid()
        {
            //Arrange
            var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            _systemUnderTest!.AddAddressesComplete = "yes";
            var logisticsLocation = new LogisticsLocationDto()
            {
                Id = new Guid()
            };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid(), false).Result).Returns(list);
            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGetRemoveEstablishment_GivenNoExistingLocations_Redirect()
        {
            //Arrange
            var list = new List<LogisticsLocationDto> { };
            _systemUnderTest!.AddAddressesComplete = "yes";
            var logisticsLocation = new LogisticsLocationDto()
            {
                Id = new Guid()
            };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync(logisticsLocation.Id)).ReturnsAsync(logisticsLocation);
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(logisticsLocation));
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid(), false).Result).Returns(list);

            //Act
            await _systemUnderTest.OnGetRemoveEstablishment(new Guid(), new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }


        [Test]
        public void OnGetChangeEstablishmentAddress_SubmitIsValid()
        {
            //Arrange
            _systemUnderTest!.AddAddressesComplete = "yes";

            //Act
            _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public void OnGetChangeEstablishmentAddress_GivenAddedManually_SubmitIsValid()
        {
            //Arrange
            var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            _systemUnderTest!.AddAddressesComplete = "yes";

            //Act
            _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public void OnGetChangeEstablishmentAddress_GivenNotAddedManually_Redirect()
        {
            //Arrange
            var list = new List<LogisticsLocationDto> { new LogisticsLocationDto() };
            _systemUnderTest!.AddAddressesComplete = "yes";

            //Act
            _systemUnderTest.OnGetChangeEstablishmentAddress(new Guid(), new Guid());
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public void OnGetChangeEmail_Redirect_Successfully()
        {
            var orgId = Guid.NewGuid();
            var establishmentId = Guid.NewGuid();
            string NI_GBFlag = "GB";
            var expected = new RedirectToPageResult(
                Routes.Pages.Path.EstablishmentContactEmailPath, 
                new { id = orgId, locationId = establishmentId, NI_GBFlag});

            // Act
            var result = _systemUnderTest?.OnGetChangeEmail(orgId, establishmentId, NI_GBFlag);

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
            var expectedHeading = "Places of destination";
            var expectedContentText = "destination";

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
            _systemUnderTest.ContentText.Should().Be(expectedContentText);
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

        [Test]
        public async Task OnGet_PracticeName_ShouldBeSet_Successfully()
        {
            //Arrange
            var tradeParty = new TradePartyDto()
            {
                Id = Guid.NewGuid(),
                PracticeName = "Practice Ltd"
            };

            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<TradePartyDto>())).Returns(false);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid(), false).Result).Returns(new List<LogisticsLocationDto>());
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).ReturnsAsync(tradeParty);

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "NI");

            //Assert
            _systemUnderTest.PracticeName.Should().Be(tradeParty.PracticeName);
        }
    }
}
