using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Constants;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.TaskList;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.AspNetCore.Authentication;
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
        protected Mock<ILogger<RegistrationTaskListModel>> _mockLogger = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegistrationTaskListModel(_mockLogger.Object, _mockTraderService.Object, _mockEstablishmentService.Object);
        }

        [Test]
        public async Task OnGet_NoCountryPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            Guid guid = Guid.NewGuid();

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

            var tradeContact = new TradeContactDTO
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890"
            };

            var tradeAddress = new TradeAddressDTO
            {
                TradeCountry = "Test Country",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDTO
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
        public async Task OnGet_GivenLogisticsLocationDetailsProvided_MarkPointsOfDepartureComplete()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDTO
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890"
            };

            var tradeAddress = new TradeAddressDTO
            {
                TradeCountry = "Test Country",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDTO
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                PartyName = "Test",
                NatureOfBusiness = "Test nature"
            };

            var list = new List<LogisticsLocationDTO> 
            { 
                new LogisticsLocationDTO() { NI_GBFlag = "NI"}
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(guid)).Returns(Task.FromResult(list.AsEnumerable())!);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.PointsOfDeparture.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.PointsOfDestination.Should().Be(TaskListStatus.COMPLETE);
        }

        [Test]
        public async Task GetAPIData_GivenCountryAndFboIsPopulated_EligibilityMarkedAsComplete()
        {
            //Arrange
            TradePartyDTO tradePartyFromApi = new()
            {
                Id = Guid.NewGuid(),
                FboNumber = "fbonum-123456-fbonum",
                Address = new TradeAddressDTO { Id = Guid.NewGuid(), TradeCountry = "GB"}
            };
            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyFromApi);

            //Act
            await _systemUnderTest!.GetAPIData();

            //Assert
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.COMPLETE);
        }

        [Test]
        public async Task GetAPIData_GivenFboNotPopulated_EligibilityMarkedAsNotStarted()
        {
            //Arrange
            TradePartyDTO tradePartyFromApi = new()
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDTO { Id = Guid.NewGuid(), TradeCountry = "GB" }
            };
            _mockTraderService
                .Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyFromApi);

            //Act
            await _systemUnderTest!.GetAPIData();

            //Assert
            _systemUnderTest.EligibilityStatus.Should().Be(TaskListStatus.NOTSTART);
        }

        [Test]
        public async Task OnGet_StatusTestsAllComplete()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDTO
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

            var tradeAddress = new TradeAddressDTO
            {
                TradeCountry = "NI",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDTO
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                AuthorisedSignatory = authorisedSignatory,
                PartyName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature"
            };

            var list = new List<LogisticsLocationDTO>
            {
                new LogisticsLocationDTO() { NI_GBFlag = "NI"}
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
            _systemUnderTest.AuthorisedSignatoryDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.PointsOfDeparture.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.PointsOfDestination.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.NOTSTART);
        }

        [Test]
        public async Task OnGet_StatusTestsNotAllComplete_NoReviewAnswers()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDTO
            {
                PersonName = "Test Name",
                Email = "test@testmail.com",
                Position = "Main Tester",
                TelephoneNumber = "1234567890",
                IsAuthorisedSignatory = false
            };

            var tradeAddress = new TradeAddressDTO
            {
                TradeCountry = "Test Country",
                LineOne = "1 Test Lane",
                PostCode = "12345"
            };

            var tradePartyDto = new TradePartyDTO
            {
                Id = guid,
                Contact = tradeContact,
                Address = tradeAddress,
                PartyName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature"
            };

            var list = new List<LogisticsLocationDTO>
            {
                new LogisticsLocationDTO() { NI_GBFlag = "NI"}, new LogisticsLocationDTO() { NI_GBFlag = "GB"}
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
            _systemUnderTest.PointsOfDeparture.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.PointsOfDestination.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.CANNOTSTART);
        }

        [Test]
        public async Task OnGet_AuthorisedSignatoryIsContact()
        {
            //Arrange
            Guid guid = Guid.NewGuid();

            var tradeContact = new TradeContactDTO
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

            var tradePartyDto = new TradePartyDTO
            {
                Id = guid,
                Contact = tradeContact,
                PartyName = "Test",
                FboNumber = "123",
                NatureOfBusiness = "Test nature"
            };


            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.ContactDetails.Should().Be(TaskListStatus.COMPLETE);
            _systemUnderTest.AuthorisedSignatoryDetails.Should().Be(TaskListStatus.NOTSTART);
            _systemUnderTest.ReviewAnswers.Should().Be(TaskListStatus.CANNOTSTART);
        }
    }
}
