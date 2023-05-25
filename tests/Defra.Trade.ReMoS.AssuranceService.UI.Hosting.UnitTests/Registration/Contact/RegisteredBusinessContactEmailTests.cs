using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Contact
{
    [TestFixture]
    public class RegisteredBusinessContactEmailTests : PageModelTestsBase
    {
        private RegisteredBusinessContactEmailModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessContactEmailModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessContactEmailModel(_mockLogger.Object, _mockTraderService.Object);
        }

        [Test]
        public async Task OnGet_NoEmailPresentIfNoSavedData()
        {
            //Arrange
            Guid test = Guid.NewGuid();

            //Act
            await _systemUnderTest!.OnGetAsync(test);

            //Assert
            _systemUnderTest.Email.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidEmail()
        {
            //Arrange
            _systemUnderTest!.Email = "Business-test@email.com";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidEmailNotPresent()
        {
            //Arrange
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter the email address of the contact person";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidRegex()
        {
            //Arrange
            _systemUnderTest!.Email = "test at email.com";
            var expectedResult = "Enter an email address in the correct format, like name@example.com";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidLength()
        {
            //Arrange
            _systemUnderTest!.Email = $"{new string('a', 100)}@email.com";
            var expectedResult = "Email is too long";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidInput()
        {
            //Arrange
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter the email address of the contact person";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");


            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnGetAsync_GuidLookupFillsInGapsForProperties()
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
            _systemUnderTest!.Email = "Business-test@email.com";

            //Act
            await _systemUnderTest.OnGetAsync(guid);
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
        }
    }
}
