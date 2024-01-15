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
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("8d455cbf-7d1f-403e-a6d3-a1275bb3ecf8") });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
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
            var expectedResult = "Enter an email address in the correct format, like name@example.com";

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
            var expectedResult = "Enter an email address in the correct format, like name@example.com";

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

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task OnPostSave_SubmitInvalidLength()
        {
            //Arrange
            _systemUnderTest!.Email = $"{new string('a', 100)}@email.com";

            //Act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInvalidInput()
        {
            //Arrange
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";
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
            _systemUnderTest!.Email = "";
            var expectedResult = "Enter an email address";
            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");

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
            var guid = Guid.Parse("8d455cbf-7d1f-403e-a6d3-a1275bb3ecf8");

            var tradeContact = new TradeContactDto() { PersonName = "Test1" };

            var tradePartyDto = new TradePartyDto
            {
                Id = guid,
                Contact = tradeContact,
                PracticeName = "Test Ltd",

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
        public async Task OnPostSave_PopulateAuthRepWhenChangeEmail()
        {
            // arrange
            _systemUnderTest!.Email = "test@test.com";
            _systemUnderTest.PracticeName = "ACME Ltd";
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto
                {
                    IsAuthorisedSignatory = true,
                },
                AuthorisedSignatory = new AuthorisedSignatoryDto
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = "testold@test.com"
                }
            };
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradeParty);

            // act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            // assert
            validation.Count.Should().Be(0);
            _systemUnderTest.AuthorisedSignatoryId.Should().Be(tradeParty.AuthorisedSignatory.Id);
            _systemUnderTest.TradePartyDto.Contact!.Email.Should().Be("test@test.com");
            _systemUnderTest.TradePartyDto.Contact.Email.Should().Be(_systemUnderTest.TradePartyDto.AuthorisedSignatory!.EmailAddress);
            _systemUnderTest.AuthorisedSignatoryId.Should().Be(_systemUnderTest.TradePartyDto.AuthorisedSignatory.Id);
        }

        [Test]
        public async Task OnPostSave_NotPopulateAuthRepWhenChangeEmail()
        {
            // arrange
            _systemUnderTest!.Email = "test@test.com";
            _systemUnderTest.PracticeName = "ACME Ltd";
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto
                {
                    IsAuthorisedSignatory = false,
                },
                AuthorisedSignatory = new AuthorisedSignatoryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "testold@test.com"
                }
            };
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradeParty);

            // act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            // assert
            validation.Count.Should().Be(0);
            _systemUnderTest.AuthorisedSignatoryId.Should().Be(tradeParty.AuthorisedSignatory.Id);
            _systemUnderTest.TradePartyDto.Contact!.Email.Should().Be("test@test.com");
            _systemUnderTest.TradePartyDto.AuthorisedSignatory.Should().BeNull();
        }

        [Test]
        public async Task OnPostSave_NotCompletedAuthRep()
        {
            // arrange
            _systemUnderTest!.Email = "test@test.com";
            _systemUnderTest.PracticeName = "ACME Ltd";
            var tradeParty = new TradePartyDto
            {
                Contact = new TradeContactDto
                {
                    IsAuthorisedSignatory = null,
                }
            };
            _mockTraderService.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradeParty);

            // act
            await _systemUnderTest.OnPostSaveAsync();
            var validation = ValidateModel(_systemUnderTest);

            // assert
            validation.Count.Should().Be(0);
            _systemUnderTest.AuthorisedSignatoryId.Should().Be(Guid.Empty);
            _systemUnderTest.TradePartyDto.Contact!.Email.Should().Be("test@test.com");
            _systemUnderTest.TradePartyDto.AuthorisedSignatory.Should().BeNull();
        }
    }
}
