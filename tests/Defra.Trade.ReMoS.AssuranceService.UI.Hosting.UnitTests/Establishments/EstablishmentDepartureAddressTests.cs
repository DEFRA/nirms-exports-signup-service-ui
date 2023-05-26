using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class EstablishmentDepartureAddressTests : PageModelTestsBase
    {
        private EstablishmentDepartureAddressModel? _systemUnderTest;
        protected Mock<ILogger<EstablishmentDepartureAddressModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new EstablishmentDepartureAddressModel(_mockLogger.Object, _mockEstablishmentService.Object);
        }

        [Test]
        public async Task OnGet_NoAddressPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced

            //Act
            await _systemUnderTest!.OnGetAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));

            //Assert
            _systemUnderTest.EstablishmentName.Should().Be("");
            _systemUnderTest.LineOne.Should().Be("");
            _systemUnderTest.LineTwo.Should().Be("");
            _systemUnderTest.CityName.Should().Be("");
            _systemUnderTest.Country.Should().Be("");
            _systemUnderTest.PostCode.Should().Be("");
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidAddress()
        {
            //Arrange
            _systemUnderTest!.EstablishmentName = "Test name";
            _systemUnderTest!.LineOne = "Line one";
            _systemUnderTest!.LineTwo = "Line two";
            _systemUnderTest!.CityName = "City";
            _systemUnderTest!.Country = "UK";
            _systemUnderTest!.PostCode = "TES1";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidRadio()
        {
            //Arrange
            _systemUnderTest!.EstablishmentName = "";
            _systemUnderTest!.LineOne = "";
            _systemUnderTest!.CityName = "";
            _systemUnderTest!.PostCode = "";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(4);
        }

        [Test]
        public async Task OnPostSubmit_SubmitInValidRadio_GetsTradePartyData()
        {
            //Arrange
            _systemUnderTest!.EstablishmentName = "";
            _systemUnderTest!.LineOne = "";
            _systemUnderTest!.CityName = "";
            _systemUnderTest!.PostCode = "";

            _systemUnderTest.ModelState.AddModelError(string.Empty, "There is something wrong with input");
   
            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(4);
        }
    }
}
