using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
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
    }
}
