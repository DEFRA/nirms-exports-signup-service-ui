using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.Pages.Establishments;
using Microsoft.Extensions.Logging;
using Moq;
using Defra.Trade.ReMoS.AssuranceService.UI.Hosting.UnitTests.Shared;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;

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
            var logisticsLocations = new List<LogisticsLocation>
            {
                new LogisticsLocation()
                {
                    Name = "Test 2",
                    Id = Guid.NewGuid(),
                    Address = new TradeAddress()
                    {
                        LineOne = "line 1",
                        CityName = "city",
                        PostCode = "TES1",
                    }
                }
            };
            var id = Guid.NewGuid();
            var postcode = "TES1";

            _mockEstablishmentService.Setup(x => x.GetLogisticsLocationByPostcodeAsync(postcode).Result).Returns(logisticsLocations);

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

            var logisticsLocations = new LogisticsLocationRelationshipDTO
            {
                TraderId = Guid.NewGuid(),
                EstablishmentId = Guid.NewGuid()
            };

            _mockEstablishmentService.Setup(x => x.AddLogisticsLocationRelationshipAsync(logisticsLocations).Result).Returns(logisticsLocations.EstablishmentId);

            //Act
            await _systemUnderTest.OnPostSubmitAsync();
            var validation = ValidateModel(_systemUnderTest);

            //Assert
            validation.Count.Should().Be(0);
        }

        [Test]
        public async Task OnPostSubmitAsync_ShouldAddAddress()
        {
            // TODO add test
        }
    }
}
