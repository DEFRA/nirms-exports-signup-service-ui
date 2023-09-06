using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class PostcodeResultsTests : PageModelTestsBase
    {
        private PostcodeResultModel? _systemUnderTest;
        protected Mock<ILogger<PostcodeResultModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();
        protected Mock<ITraderService> _mockTraderService = new();        

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new PostcodeResultModel(_mockLogger.Object, _mockEstablishmentService.Object, _mockTraderService.Object)
            {
                PageContext = PageModelMockingUtils.MockPageContext()
            };
        }

        [Test]
        public async Task OnGetAsync_ReturnsLogisticsLocations()
        {
            // arrange
            var logisticsLocations = new List<LogisticsLocationDto>
            {
                new LogisticsLocationDto()
                {
                    Name = "Test 2",
                    Id = Guid.NewGuid(),
                    NI_GBFlag = "GB",
                    Address = new TradeAddressDto()
                    {
                        LineOne = "line 1",
                        CityName = "city",
                        PostCode = "TES1",
                    }
                }
            };
            var id = Guid.NewGuid();
            var postcode = "TES1";

            _mockEstablishmentService.Setup(x => x.GetEstablishmentByPostcodeAsync(postcode).Result).Returns(logisticsLocations);
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(false);
            // act
            await _systemUnderTest!.OnGetAsync(id, postcode);

            // assert
            _systemUnderTest.EstablishmentsList.Should().HaveCount(1);
            _systemUnderTest.EstablishmentsList![0].Text.Should().Be("Test 2, line 1, city, TES1");
            _systemUnderTest.EstablishmentsList[0].Value.Should().Be(logisticsLocations[0].Id.ToString());
            }

        [Test]
        public async Task OnPostSubmitAsync_ShouldBeValid()
        {
            //Arrange
            _systemUnderTest!.Postcode = "TES1";
            _systemUnderTest!.TradePartyId = Guid.NewGuid();
            _systemUnderTest!.SelectedEstablishment = Guid.NewGuid().ToString();

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmitAsync_GivenInvalidModdel_ShouldBeHandled()
        {
            //Arrange
            _systemUnderTest!.Postcode = "TES1";
            _systemUnderTest!.TradePartyId = Guid.NewGuid();
            _systemUnderTest!.SelectedEstablishment = Guid.NewGuid().ToString();

            _systemUnderTest!.ModelState.AddModelError("TestError", "Something broke");

            var logisticsLocations = new LogisticsLocationDto
            {
                TradePartyId = _systemUnderTest!.TradePartyId,
                Id = Guid.NewGuid()
            };
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnGet_HeadingSetToParameter_Successfully()
        {
            //Arrange
            var expectedHeading = "Add a place of destination";
            var expectedContentText = "The locations in Northern Ireland which are part of your business where consignments will go after the port of entry under the scheme. You will have to provide the details for all locations, so they can be used when applying for General Certificates.";
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentByPostcodeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<List<LogisticsLocationDto>?>(new List<LogisticsLocationDto>() { new LogisticsLocationDto() }));

            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "aaa", "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
            _systemUnderTest.ContentText.Should().Be(expectedContentText);
        }

        [Test]
        public async Task OnGetAsync_InvalidOrgId()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), It.IsAny<string>());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Errors/AuthorizationError");
        }

        [Test]
        public async Task OnGetAsync_RedirectRegisteredBusiness()
        {
            _mockTraderService.Setup(x => x.ValidateOrgId(_systemUnderTest!.User.Claims, It.IsAny<Guid>())).ReturnsAsync(true);
            _mockTraderService.Setup(x => x.IsTradePartySignedUp(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await _systemUnderTest!.OnGetAsync(Guid.NewGuid(), It.IsAny<string>());
            var redirectResult = result as RedirectToPageResult;

            redirectResult!.PageName.Should().Be("/Registration/RegisteredBusiness/RegisteredBusinessAlreadyRegistered");
        }
    }
}
