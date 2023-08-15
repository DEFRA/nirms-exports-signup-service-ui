using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class EstablishmentNameAndAddressTests : PageModelTestsBase
    {
        private EstablishmentNameAndAddressModel? _systemUnderTest;
        protected Mock<ILogger<EstablishmentNameAndAddressModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();
        protected Mock<ITraderService> _mockTraderService = new();        

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new EstablishmentNameAndAddressModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object);
            _systemUnderTest.PageContext = PageModelMockingUtils.MockPageContext();
        }

        [Test]
        public async Task OnGet_NoAddressPresentIfNoSavedData()
        {
            //Arrange
            //TODO: Add setup for returning values when API referenced
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            //Act
            await _systemUnderTest!.OnGetAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"), Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));

            //Assert
            _systemUnderTest.EstablishmentName.Should().Be("");
            _systemUnderTest.LineOne.Should().Be("");
            _systemUnderTest.LineTwo.Should().Be("");
            _systemUnderTest.CityName.Should().Be("");
            _systemUnderTest.County.Should().Be("");
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
            _systemUnderTest!.County = "Berkshire";
            _systemUnderTest!.PostCode = "EC1N 2PB";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidAddress_DuplicateSpotted()
        {
            //Arrange

            var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
                Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            _systemUnderTest!.EstablishmentId = Guid.NewGuid();
            _systemUnderTest!.EstablishmentName = "Test name";
            _systemUnderTest!.LineOne = "Line one";
            _systemUnderTest!.LineTwo = "Line two";
            _systemUnderTest!.CityName = "City";
            _systemUnderTest!.County = "Berkshire";
            _systemUnderTest!.PostCode = "TES1";
            
            //Act
            await _systemUnderTest.OnPostSubmitAsync();

            //Assert
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.HasError("EstablishmentName").Should().Be(true);
        }

        [Test]
        public async Task OnPostSubmit_SubmitValidAddress_DuplicateSpotted_FlagsChecked()
        {
            //Arrange

            var list = new List<LogisticsLocationDto> { new LogisticsLocationDto { Name = "Test name",
                Address = new TradeAddressDto { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), LineOne = "Line one", LineTwo = "Line two", CityName = "City", County = "Berkshire", PostCode = "TES1" } } };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentsForTradePartyAsync(new Guid()).Result).Returns(list);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            _systemUnderTest!.EstablishmentId = Guid.NewGuid();
            _systemUnderTest!.EstablishmentName = "Test name";
            _systemUnderTest!.LineOne = "Line one";
            _systemUnderTest!.LineTwo = "Line two";
            _systemUnderTest!.CityName = "City";
            _systemUnderTest!.County = "Berkshire";
            _systemUnderTest!.PostCode = "TES1";

            _systemUnderTest.NI_GBFlag = "NI";

            //Act
            await _systemUnderTest.OnPostSubmitAsync();

            //Assert
            _systemUnderTest.ModelState.ErrorCount.Should().Be(1);
            _systemUnderTest.ModelState.HasError("EstablishmentName").Should().Be(true);
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
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(4);
        }

        [Test]
        public async Task OnGet_HeadingSetToParameter_Successfully()
        {
            //Arrange
            var expectedHeading = "Add a place of destination";
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }
    }
}
