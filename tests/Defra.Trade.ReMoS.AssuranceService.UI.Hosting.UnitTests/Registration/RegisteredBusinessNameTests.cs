using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;
#pragma warning disable CS8602
namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration
{
    [TestFixture]
    public class RegisteredBusinessNameTests : PageModelTestsBase
    {
        private RegisteredBusinessNameModel? _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessNameModel>> _mockLogger = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new RegisteredBusinessNameModel(_mockLogger.Object);
        }

        [Test]
        public async Task OnGet_NoNamePresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest.OnGetAsync();

            //Assert
            _systemUnderTest.Name.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidName()
        {
            //Arrange
            _systemUnderTest.Name = "Business-Name1";            

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);            
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidNameNotPresent()
        {
            //Arrange
            _systemUnderTest.Name = "";
            var expectedResult = "Enter your business name";

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
            _systemUnderTest.Name = "Business%%Name1";
            var expectedResult = "Enter your business name using only letters, numbers, and special characters -_./()&";

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
            _systemUnderTest.Name = new string('a', 101);
            var expectedResult = "Business name is too long";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert            
            validation.Count.Should().Be(1);
            Assert.AreEqual(expectedResult, validation[0].ErrorMessage);
        }
    }
}
