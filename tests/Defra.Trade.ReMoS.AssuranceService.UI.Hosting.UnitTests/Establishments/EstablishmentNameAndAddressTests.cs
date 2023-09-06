using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;

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
            await _systemUnderTest!.OnGetAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"), Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"), null);

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
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), null, "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), null);
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase(null)]
        public async Task RetrieveEstablishmentDetails_ReturnsEmptyLocation(Guid? guid)
        {
            // arrange
            _systemUnderTest!.Uprn = null;
            _systemUnderTest.EstablishmentId = guid;
            
            // act
            await _systemUnderTest.RetrieveEstablishmentDetails();

            // assert
            _systemUnderTest.EstablishmentName.Should().BeEmpty();
            _systemUnderTest.LineOne.Should().BeEmpty();
            _systemUnderTest.LineTwo.Should().BeEmpty();
            _systemUnderTest.CityName.Should().BeEmpty();
            _systemUnderTest.PostCode.Should().BeEmpty();
            _systemUnderTest.County.Should().BeEmpty();
        }

        [Test]
        public async Task RetrieveEstablishmentDetails_ReturnsTradeApiAddress()
        {
            // arrange
            _systemUnderTest!.Uprn = "1234";
            var establishment = new LogisticsLocationDto()
            {
                Name = "business name",
                Address = new TradeAddressDto()
                {
                    LineOne = "line 1",
                    LineTwo = "lines 2",
                    PostCode = "postcode",
                    CityName = "city",
                    County = "county"
                }
            };
            _mockEstablishmentService.Setup(x => x.GetLogisticsLocationByUprnAsync(_systemUnderTest.Uprn)).ReturnsAsync(establishment);

            // act
            await _systemUnderTest.RetrieveEstablishmentDetails();

            // assert
            _systemUnderTest.EstablishmentName.Should().Be(establishment.Name);
            _systemUnderTest.LineOne.Should().Be(establishment.Address.LineOne);
            _systemUnderTest.LineTwo.Should().Be(establishment.Address.LineTwo);
            _systemUnderTest.CityName.Should().Be(establishment.Address.CityName);
            _systemUnderTest.PostCode.Should().Be(establishment.Address.PostCode);
            _systemUnderTest.County.Should().Be(establishment.Address.County);
        }

        [Test]
        public async Task RetrieveEstablishmentDetails_ReturnsAddress()
        {
            // arrange
            _systemUnderTest!.EstablishmentId = Guid.NewGuid();
            var establishment = new LogisticsLocationDto()
            {
                Name = "business name",
                Address = new TradeAddressDto()
                {
                    LineOne = "line 1",
                    LineTwo = "lines 2",
                    PostCode = "postcode",
                    CityName = "city",
                    County = "county"
                }
            };
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync((Guid)_systemUnderTest.EstablishmentId!)).ReturnsAsync(establishment);

            // act
            await _systemUnderTest.RetrieveEstablishmentDetails();

            // assert
            _systemUnderTest.EstablishmentName.Should().Be(establishment.Name);
            _systemUnderTest.LineOne.Should().Be(establishment.Address.LineOne);
            _systemUnderTest.LineTwo.Should().Be(establishment.Address.LineTwo);
            _systemUnderTest.CityName.Should().Be(establishment.Address.CityName);
            _systemUnderTest.PostCode.Should().Be(establishment.Address.PostCode);
            _systemUnderTest.County.Should().Be(establishment.Address.County);
        }

        [TestCase("123", "00000000-0000-0000-0000-000000000000")]
        [TestCase(null, "00000000-0000-0000-0000-000000000000")]
        [TestCase(null, null)]
        public async Task SaveEstablishmentDetails_Create(string uprn, Guid guid)
        {
            // arrange
            var expected = Guid.NewGuid();
            _systemUnderTest!.EstablishmentName = "Test name";
            _systemUnderTest!.LineOne = "Line one";
            _systemUnderTest!.LineTwo = "Line two";
            _systemUnderTest!.CityName = "City";
            _systemUnderTest!.County = "Berkshire";
            _systemUnderTest!.PostCode = "TES1";
            _systemUnderTest!.Uprn = uprn;
            _systemUnderTest.EstablishmentId = guid;
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            _mockEstablishmentService.Setup(x => x.CreateEstablishmentForTradePartyAsync(_systemUnderTest.TradePartyId, It.IsAny<LogisticsLocationDto>())).ReturnsAsync(expected);

            // act
            var result = await _systemUnderTest.SaveEstablishmentDetails();

            // assert
            result.Should().Be(expected);
            _mockEstablishmentService.Verify(x => x.CreateEstablishmentForTradePartyAsync(_systemUnderTest.TradePartyId, It.IsAny<LogisticsLocationDto>()), Times.Once());
        }

        [Test]
        public async Task SaveEstablishmentDetails_Update()
        {
            // arrange
            var expected = Guid.NewGuid();
            _systemUnderTest!.EstablishmentName = "Test name";
            _systemUnderTest!.LineOne = "Line one";
            _systemUnderTest!.LineTwo = "Line two";
            _systemUnderTest!.CityName = "City";
            _systemUnderTest!.County = "Berkshire";
            _systemUnderTest!.PostCode = "TES1";
            _systemUnderTest.EstablishmentId = Guid.NewGuid();
            _systemUnderTest.TradePartyId = Guid.NewGuid();
            _mockEstablishmentService.Setup(x => x.GetEstablishmentByIdAsync((Guid)_systemUnderTest.EstablishmentId!)).ReturnsAsync(new LogisticsLocationDto() { Address = new TradeAddressDto()});
            _mockEstablishmentService.Setup(x => x.UpdateEstablishmentDetailsAsync(It.IsAny<LogisticsLocationDto>())).ReturnsAsync(true);

            // act
            var result = await _systemUnderTest.SaveEstablishmentDetails();

            // assert
            result.Should().Be(_systemUnderTest.EstablishmentId);
            _mockEstablishmentService.Verify(x => x.UpdateEstablishmentDetailsAsync(It.IsAny<LogisticsLocationDto>()), Times.Once());
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), Guid.NewGuid(), null);
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}
