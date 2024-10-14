using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.ValidationExtensions;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Contact
{
    [TestFixture]
    public class RegisteredBusinessContactEmailTests : PageModelTestsBase
    {
        private RegisteredBusinessContactEmailModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessContactEmailModel>> _mockLogger = new();
        protected Mock<ITraderService> _mockTraderService = new();
        private StringLengthMaximumAttribute? stringLengthMaximumAttribute { get; set; }

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessContactEmailModel(_mockLogger.Object, _mockTraderService.Object)
            {
                PageContext = PageModelMockingUtils.MockPageContext()
            };
            _mockTraderService.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TradePartyDto() { Id = Guid.Parse("8d455cbf-7d1f-403e-a6d3-a1275bb3ecf8") });
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).Returns(true);
            stringLengthMaximumAttribute = new StringLengthMaximumAttribute(100);
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
        public void OnValidate_SubmitInvalidLength()
        {
            //Arrange
            _systemUnderTest!.Email = $"ryan.testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@email.com@email.com";

            //Act
            var validation = stringLengthMaximumAttribute!.IsValid(_systemUnderTest!.Email);

            //Assert
            validation.Should().Be(false);
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
            validation.Count.Should().Be(0);
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