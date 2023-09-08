using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.EventHub.Fluent.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.TaskList
{
    [TestFixture]
    public class RegistratonTaskListTests : PageModelTestsBase
    {
        private RegistrationTaskListModel? _systemUnderTest;
        private readonly Mock<ITraderService> _mockTraderService = new();
        private readonly Mock<IEstablishmentService> _mockEstablishmentService = new();        
        private readonly ICheckAnswersService _checkAnswersService = new CheckAnswersService();
        protected Mock<ILogger<RegistrationTaskListModel>> _mockLogger = new();        

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegistrationTaskListModel(_mockLogger.Object, _mockTraderService.Object, _mockEstablishmentService.Object, _checkAnswersService);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_NoCountryPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            Guid guid = Guid.NewGuid();
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);            

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert

            _systemUnderTest.RegistrationID.Should().NotBe(Guid.Empty);
        }

        [Test]
        public async Task OnGet_NoCountryPresentIfNoSavedData_CheckIfTradePartyIsNull()
        {
            //Arrange
            Guid guid = Guid.NewGuid();
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            var tradeContact = new TradeContactDto
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890"
            };

            var tradeAddress = new TradeAddressDto
            {
                TradeCountry = "Test Country",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                PartyName = "Test",
                NatureOfBusiness = "Test nature"
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.RegistrationID.Should().NotBe(Guid.Empty);
        }

        [Test]
        public async Task OnGet_GivenLogisticsLocationDetailsProvided_MarkPlacesOfDispatchComplete()
        {
            //Arrange
            Guid guid = Guid.NewGuid();            

            var tradeContact = new TradeContactDto
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890"
            };

            var tradeAddress = new TradeAddressDto
            {
                TradeCountry = "NI",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                PartyName = "Test",
                NatureOfBusiness = "Test nature"
            };

            var list = new List<LogisticsLocationDto>
            {
                new LogisticsLocationDto() { NI_GBFlag = "NI"}
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(guid)).Returns(Task.FromResult(list.AsEnumerable())!);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.EstablishmentsAdded.Should().Be(true);
            _systemUnderTest.PlacesOfDispatch.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.PlacesOfDestination.Should().Be(TaskListStatus.COMPLETE);
        }

        [Test]
        public async Task GetAPIData_GivenCountryAndFboIsPopulated_EligibilityMarkedAsComplete()
        {
            //Arrange
            TradePartyDto tradePartyFromApi = new()
            {
                Id = Guid.NewGuid(),
                FboNumber = "fbonum-123456-fbonum",
                Address = new TradeAddressDto { Id = Guid.NewGuid(), TradeCountry = "GB" }
            };
            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyFromApi);

            //Act
            await _systemUnderTest!.GetAPIData();

            //Assert
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.INPROGRESS);
        }

        [Test]
        public async Task GetAPIData_GivenFboNotPopulated_EligibilityMarkedAsNotStarted()
        {
            //Arrange
            TradePartyDto tradePartyFromApi = new()
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto { Id = Guid.NewGuid(), TradeCountry = "GB" }
            };
            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyFromApi);

            //Act
            await _systemUnderTest!.GetAPIData();

            //Assert
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.INPROGRESS);
        }

        [Test]
        public async Task OnGet_StatusTestsAllComplete()
        {
            //Arrange
            Guid guid = Guid.NewGuid();
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            var tradeContact = new TradeContactDto
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890",
                IsAuthorisedSignatory = false
            };

            var authorisedSignatory = new AuthorisedSignatoryDto
            {
                Name = "Test",
                EmailAddress = "Test",
                Id = Guid.NewGuid(),
                Position = "CEO",
                TradePartyId = guid
            };

            var tradeAddress = new TradeAddressDto
            {
                TradeCountry = "GB",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                AuthorisedSignatory = authorisedSignatory,
                PartyName = "Test",
                PracticeName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature",
                RegulationsConfirmed = true
            };

            var list = new List<LogisticsLocationDto>
            {
                new LogisticsLocationDto() { NI_GBFlag = "GB"}
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(guid)).Returns(Task.FromResult(list.AsEnumerable())!);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.EstablishmentsAdded.Should().Be(true);
            _systemUnderTest.BusinessDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ContactDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.AuthorisedSignatoryDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.PlacesOfDispatch.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.PlacesOfDestination.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.NOTSTART);
        }

        [Test]
        public async Task OnGet_StatusTestsNotAllComplete_NoReviewAnswers()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDto
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890",
                IsAuthorisedSignatory = null
            };

            var tradeAddress = new TradeAddressDto
            {
                TradeCountry = "Test Country",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                PartyName = "Test",
                PracticeName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature",
                RegulationsConfirmed = true
            };

            var list = new List<LogisticsLocationDto>
            {
                new LogisticsLocationDto() { NI_GBFlag = "NI"}, new LogisticsLocationDto() { NI_GBFlag = "GB"}
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(guid)).Returns(Task.FromResult(list.AsEnumerable())!);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.BusinessDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ContactDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.AuthorisedSignatoryDetails.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.PlacesOfDispatch.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.PlacesOfDestination.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.CANNOTSTART);
        }

        [Test]
        public async Task OnGet_AuthorisedSignatoryIsContact()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDto
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890",
                IsAuthorisedSignatory = true
            };

            var authorisedSignatory = new AuthorisedSignatoryDto
            {
                Name = "Test",
                EmailAddress = "Test",
                Id = Guid.NewGuid(),
                Position = "CEO",
                TradePartyId = guid
            };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                PartyName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature",
                RegulationsConfirmed = true
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.ContactDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.AuthorisedSignatoryDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.CANNOTSTART);
        }

        [Test]
        public void GetBusinessDetailsProgress_Status_InProgress()
        {
            // Arrange
            var tradeParty = new TradePartyDto { PartyName = "Test", PracticeName = "Test" };
            var expectedStatus = TaskListStatus.INPROGRESS;

            var status = _systemUnderTest!.GetBusinessDetailsProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        public void GetBusinessDetailsProgress_Status_NotStarted()
        {
            // Arrange
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto() { IsAuthorisedSignatory = false },
            };
            var expectedStatus = TaskListStatus.NOTSTART;

            var status = _systemUnderTest!.GetBusinessDetailsProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        public void GetContactDetailsProgress_Status_InProgress()
        {
            // Arrange
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto() { PersonName = "Test" }
            };

            var expectedStatus = TaskListStatus.INPROGRESS;

            var status = _systemUnderTest!.GetContactDetailsProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        public void GetContactDetailsProgress_Status_NotStarted()
        {
            // Arrange
            var tradeParty = new TradePartyDto();

            var expectedStatus = TaskListStatus.NOTSTART;

            var status = _systemUnderTest!.GetContactDetailsProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [TestCase(false, "TestName", null, null, TaskListStatus.INPROGRESS)]
        [TestCase(false, null, "TestPosition", null, TaskListStatus.INPROGRESS)]
        [TestCase(false, null, null, "TestEmail", TaskListStatus.INPROGRESS)]
        [TestCase(true, "TestName", "TestPosition", "TestEmail", TaskListStatus.COMPLETE)]
        [TestCase(false, "TestName", "TestPosition", "TestEmail", TaskListStatus.COMPLETE)]
        public void GetAuthorisedSignatoryProgress_Status_InProgressOrComplete(bool isAuthSig, string? name, string? position, string? email, string expectedStatus)
        {
            // Arrange
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto()
                {
                    PersonName = "Test",
                    Email = "Email",
                    Position = "Position",
                    TelephoneNumber = "12345678901",
                    IsAuthorisedSignatory = isAuthSig
                },
                AuthorisedSignatory = new AuthorisedSignatoryDto()
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Position = position,
                    EmailAddress = email
                }
            };

            var status = _systemUnderTest!.GetAuthorisedSignatoryProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [TestCase(null, null, null, null, TaskListStatus.NOTSTART)]
        public void GetAuthorisedSignatoryProgress_Status_InProgressOrComplete_Id_IsNull(bool? isAuthSig, string? name, string? position, string? email, string expectedStatus)
        {
            // Arrange
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto()
                {
                    PersonName = "Test",
                    Email = "Email",
                    Position = "Position",
                    TelephoneNumber = "12345678901",
                    IsAuthorisedSignatory = isAuthSig
                },
                AuthorisedSignatory = new AuthorisedSignatoryDto()
                {
                    Id = Guid.Empty,
                    Name = name,
                    Position = position,
                    EmailAddress = email
                }
            };

            var status = _systemUnderTest!.GetAuthorisedSignatoryProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        public void GetAuthorisedSignatoryProgress_Status_NoStarted()
        {
            // Arrange
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto()
                {
                    PersonName = "Test",
                    Email = "Email",
                    Position = "Position",
                    TelephoneNumber = "12345678901"
                }
            };

            var expectedStatus = TaskListStatus.NOTSTART;

            var status = _systemUnderTest!.GetAuthorisedSignatoryProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
        }

        [Test]
        public void GetAuthorisedSignatoryProgress_status_CannotStart()
        {
            // Arrange
            var tradeParty = new TradePartyDto();

            var expectedStatus = TaskListStatus.CANNOTSTART;

            var status = _systemUnderTest!.GetAuthorisedSignatoryProgress(tradeParty);

            Assert.AreEqual(expectedStatus, status);
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