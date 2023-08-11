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
    public class RegisteredBusinessContactEmailTests : PageModelTestsBase
    {
        private RegisteredBusinessContactEmailModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessContactEmailModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();        

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessContactEmailModel(_mockLogger.Object, _mockTraderService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_NoEmailPresentIfNoSavedData()
        {
            //Arrange
            Guid test = Guid.NewGuid();
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

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
        public async Task OnPostSave_SubmitValidEmail()
        {
            //Arrange
            _systemUnderTest!.Email = "Business-test@email.com";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidEmailNotPresent()
        {
            //Arrange
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSave_SubmitInValidEmailNotPresent()
        {
            //Arrange
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidRegex()
        {
            //Arrange
            _systemUnderTest!.Email = "test at email.com";
            var expectedResult = "Enter an email address using only letters, numbers, brackets, full stops, commas (,), hyphens (-), underscores (_), slashes (/), ampersands (&), (') apostrophe or (@) at sign";

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
            _systemUnderTest!.Email = "test at email.com";
            var expectedResult = "Enter an email address using only letters, numbers, brackets, full stops, commas (,), hyphens (-), underscores (_), slashes (/), ampersands (&), (') apostrophe or (@) at sign";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
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
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSave_SubmitInvalidLength()
        {
            //Arrange
            _systemUnderTest!.Email = $"{new string('a', 100)}@email.com";
            var expectedResult = "Email is too long";

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
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

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
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            expectedResult.Should().Be(validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnGetAsync_GuidLookupFillsInGapsForProperties()
        {
            //Arrange
            var guid = new Guid();

            var tradeContact = new TradeContactDto();

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact
            };

            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Verifiable();
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(guid)).Returns(Task.FromResult(tradePartyDto)!);
            _systemUnderTest!.Email = "Business-test@email.com";
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest.OnGetAsync(guid);
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }
    }
}
