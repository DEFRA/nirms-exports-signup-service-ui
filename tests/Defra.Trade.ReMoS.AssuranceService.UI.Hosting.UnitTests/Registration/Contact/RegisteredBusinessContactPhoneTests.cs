using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Contact
{
    [TestFixture]
    public class RegisteredBusinessContactPhoneTests : PageModelTestsBase
    {
        private RegisteredBusinessContactPhoneModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessContactPhoneModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessContactPhoneModel(_mockLogger.Object, _mockTraderService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("8d455cbf-7d1f-403e-a6d3-a1275bb3ecf8") });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
        }

        [Test]
        public async Task OnGet_NoPhoneNumberPresentIfNoSavedData()
        {
            //Arrange
            Guid test = Guid.NewGuid();

            //Act
            await _systemUnderTest!.OnGetAsync(test);

            //Assert
            _systemUnderTest.PhoneNumber.Should().Be("");
        }

        [Test]
        [TestCase("+44 7343 242 980")]
        [TestCase("07343 242 980")]
        [TestCase("07343242980")]
        [TestCase("+44 1234 567890")]
        public async Task OnPostSubmit_SubmitValidPhoneNumber(string phoneNumber)
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = phoneNumber;

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        public async Task OnPostSave_SubmitValidPhoneNumber(string phoneNumber)
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = phoneNumber;

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidRegex()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "07343 242 9802";
            var expectedResult = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSave_SubmitInvalidRegex()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "07343 242 9802";
            var expectedResult = "Enter a telephone number, like 01632 960 001, 07700 900 982 or +44 808 157 0192";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValid_PhoneNumberNotPresent()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "";
            var expectedResult = "Enter a telephone number";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSave_SubmitInValid_PhoneNumberNotPresent()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "";
            var expectedResult = "Enter a telephone number";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidInput()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "";
            var expectedResult = "Enter a telephone number";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSave_SubmitInvalidInput()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "";
            var expectedResult = "Enter a telephone number";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        [TestCase("+44 7343 242 980")]
        [TestCase("07343 242 980")]
        [TestCase("07343242980")]
        [TestCase("+44 1234 567890")]
        public async Task OnGetAsync_GuidLookupFillsInGapsForProperties(string phoneNumber)
        {
            //Arrange
            var guid = Guid.Parse("8d455cbf-7d1f-403e-a6d3-a1275bb3ecf8");

            var tradeContact = new TradeContactDto() { PersonName = "Test1" };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                PracticeName = "ACME Ltd"
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _systemUnderTest!.PhoneNumber = phoneNumber;

            //Act
            await _systemUnderTest.OnGetAsync(guid);
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
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
