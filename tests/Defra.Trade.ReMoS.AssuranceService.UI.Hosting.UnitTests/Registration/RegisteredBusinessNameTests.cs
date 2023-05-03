using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Registration.RegisteredBusiness;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Registration
{
    public class RegisteredBusinessNameTests : PageModelTestsBase
    {
        private RegisteredBusinessNameModel _systemUnderTest;
        protected Mock<ILogger<RegisteredBusinessNameModel>> _mockLogger = new();

        public RegisteredBusinessNameTests()
        {
            _systemUnderTest = new RegisteredBusinessNameModel(_mockLogger.Object);
        }

        [Fact]
        public async Task OnGet_NoNamePresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest.OnGetAsync();

            //Assert
            _systemUnderTest.Name.Should().Be("");
        }

        [Fact]
        public async Task OnPostSubmit_SubmitValidInformation()
        {
            //Arrange
            _systemUnderTest.Name = "";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(1);
        }
    }
}
