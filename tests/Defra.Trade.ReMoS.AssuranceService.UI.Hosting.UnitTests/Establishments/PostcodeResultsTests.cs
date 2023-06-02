using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Establishments
{
    [TestFixture]
    public class PostcodeResultsTests : PageModelTestsBase
    {
        private PostcodeResultModel? _systemUnderTest;
        protected Mock<ILogger<PostcodeResultModel>> _mockLogger = new();
        protected Mock<IEstablishmentService> _mockEstablishmentService = new();

        [SetUp]
        public void TestCaseSetup()
        {
            _systemUnderTest = new PostcodeResultModel(_mockLogger.Object, _mockEstablishmentService.Object);
        }

        [Test]
        public async Task OnGetAsync_ReturnsLogisticsLocations()
        {
            // arrange
            var logisticsLocations = new List<LogisticsLocationDTO>
            {
                new LogisticsLocationDTO()
                {
                    Name = "Test 2",
                    Id = Guid.NewGuid(),
                    Address = new TradeAddressDTO()
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

            // act
            await _systemUnderTest.OnGetAsync(id, postcode);

            // assert
            _systemUnderTest.LogisticsLocationsList.Should().HaveCount(1);
            _systemUnderTest.LogisticsLocationsList[0].Text.Should().Be("Test 2, line 1, city, TES1");
            _systemUnderTest.LogisticsLocationsList[0].Value.Should().Be(logisticsLocations[0].Id.ToString());
            }

        [Test]
        public async Task OnPostSubmitAsync_ShouldBeValid()
        {
            //Arrange
            _systemUnderTest!.Postcode = "TES1";
            _systemUnderTest!.TradePartyId = Guid.NewGuid();
            _systemUnderTest!.SelectedLogisticsLocation = Guid.NewGuid().ToString();

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = _systemUnderTest!.TradePartyId,
                LogisticsLocationId = Guid.NewGuid()
            };

            _mockEstablishmentService.Setup(x => x.AddEstablishmentToPartyAsync(logisticsLocations).Result).Returns(logisticsLocations.TradePartyId);

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
            _systemUnderTest!.SelectedLogisticsLocation = Guid.NewGuid().ToString();

            _systemUnderTest!.ModelState.AddModelError("TestError", "Something broke");

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = _systemUnderTest!.TradePartyId,
                LogisticsLocationId = Guid.NewGuid()
            };

            _mockEstablishmentService.Setup(x => x.AddEstablishmentToPartyAsync(logisticsLocations).Result).Returns(logisticsLocations.TradePartyId);

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
            var expectedHeading = "Add a point of destination (optional)";
            var expectedContentText = "Add all establishments in Northern Ireland where your goods go after the port of entry. For example, a hub or store.";
            _mockEstablishmentService
                .Setup(x => x.GetEstablishmentByPostcodeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<List<LogisticsLocationDTO>?>(new List<LogisticsLocationDTO>() { new LogisticsLocationDTO() }));

            //Act
            await _systemUnderTest!.OnGetAsync(It.IsAny<Guid>(), "aaa", "NI");

            //Assert
            _systemUnderTest.ContentHeading.Should().Be(expectedHeading);
            _systemUnderTest.ContentText.Should().Be(expectedContentText);
        }
    }
}
