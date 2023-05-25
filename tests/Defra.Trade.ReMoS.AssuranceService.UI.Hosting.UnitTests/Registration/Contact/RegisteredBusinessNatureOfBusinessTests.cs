using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Contact
{
    [TestFixture]
    public class RegisteredBusinessNatureOfBusinessTests : PageModelTestsBase
    {
        private RegisteredBusinessNatureOfBusinessModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessNatureOfBusinessModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessNatureOfBusinessModel(_mockLogger.Object, _mockTraderService.Object);
        }

        [Test]
        public async Task OnGet_NoNatureOfBusinessPresentIfNoSavedData()
        {
            //Arrange
            var guid = new Guid();

            //Act
            await _systemUnderTest!.OnGetAsync(guid);

            //Assert
            _systemUnderTest.NatureOfBusiness.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidNatureOfBusiness()
        {
            //Arrange
            _systemUnderTest!.NatureOfBusiness = "Wholesale Hamster Supplies";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidNatureOfBusinessNotPresent()
        {
            //Arrange
            _systemUnderTest!.NatureOfBusiness = "";
            var expectedResult = "error message";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidInput()
        {
            //Arrange
            _systemUnderTest!.NatureOfBusiness = "";
            var expectedResult = "error message";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");


            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnGet_NoNatureOfBusinessPresentIfNoSavedData_GetTradeParty()
        {
            //Arrange
            var guid = new Guid();

            var tradeContact = new TradeContactDTO();

            var tradePartyDto = new TradePartyDTO
            {
                Id = guid,
                Contact = tradeContact
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _systemUnderTest!.NatureOfBusiness = "Test";

            //Act
            await _systemUnderTest.OnGetAsync(guid);
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
        }
    }
}
