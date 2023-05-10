﻿using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness.Contact;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration.Contact 
{
    [TestFixture]
    public class RegisteredBusinessContactPhoneTests : PageModelTestsBase
    {
        private RegisteredBusinessContactPhoneModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessContactPhoneModel>> _mockLogger = new();

        [SetUp]
        public void TestCaseSetup() 
        {
            _systemUnderTest = new RegisteredBusinessContactPhoneModel(_mockLogger.Object);
        }

        [Test]
        public async Task OnGet_NoPhoneNumberPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest!.OnGetAsync();

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

        [Test]
        public async Task OnPostSubmit_SubmitInvalidRegex()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "07343 242 9802";
            var expectedResult = "Enter a telephone number in the correct format, like 01632 960 001, 07700 900 982 or +44 808 157 019";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValid_PhoneNumberNotPresent()
        {
            //Arrange
            _systemUnderTest!.PhoneNumber = "";
            var expectedResult = "Enter the phone number of the contact person";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }
    }
}
