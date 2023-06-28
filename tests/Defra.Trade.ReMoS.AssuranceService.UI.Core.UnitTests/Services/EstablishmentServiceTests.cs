using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    [TestFixture]
    public class EstablishmentServiceTests
    {
        private IEstablishmentService? _establishmentService;
        protected Mock<IAPIIntegration> _mockApiIntegration = new();

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_CreateEstablishmentForTradePartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var logisticsLocationDto = new LogisticsLocationDTO
            {
                Name = "Logistics location"
            };
            var partyId = Guid.NewGuid();

            _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(partyId, logisticsLocationDto)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(partyId, logisticsLocationDto)).Returns(Task.FromResult(expectedGuid as Guid?));

            // Act
            var returnedValue = await _establishmentService.CreateEstablishmentForTradePartyAsync(partyId, logisticsLocationDto);

            // Assert
            _mockApiIntegration.Verify();
            expectedGuid.Should().Be((Guid)returnedValue);
        }

        [Test]
        public async Task Service_Returns_LogisticsLocationDTO_When_Calling_GetEstablishmentsForTradePartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var logisticsLocationDto = new List<LogisticsLocationDTO>();

            _mockApiIntegration.Setup(x => x.GetEstablishmentsForTradePartyAsync(expectedGuid)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetEstablishmentsForTradePartyAsync(expectedGuid)).Returns(Task.FromResult(logisticsLocationDto)!);

            // Act
            var returnedValue = await _establishmentService.GetEstablishmentsForTradePartyAsync(expectedGuid);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Equal(logisticsLocationDto);
        }


        [Test]
        public async Task Service_Returns_LogisticsLocationDTO_When_Calling_GetEstablishmentByIdAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var logisticsLocationDto = new LogisticsLocationDTO
            {
                Name = "Logistics location"
            };

            _mockApiIntegration.Setup(x => x.GetEstablishmentByIdAsync(expectedGuid)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetEstablishmentByIdAsync(expectedGuid)).Returns(Task.FromResult(logisticsLocationDto)!);

            // Act
            var returnedValue = await _establishmentService.GetEstablishmentByIdAsync(expectedGuid);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(logisticsLocationDto);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_GetEstablishmentsForTradePartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new List<LogisticsLocationDTO>();
            logisticsLocations.Add(new LogisticsLocationDTO()
            {
                TradeAddressId = Guid.NewGuid()
            });
            var postcode = "TES1";

            _mockApiIntegration.Setup(x => x.GetEstablishmentsByPostcodeAsync(postcode)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetEstablishmentsByPostcodeAsync(postcode).Result).Returns(logisticsLocations);

            // Act
            var returnedValue = await _establishmentService.GetEstablishmentByPostcodeAsync(postcode);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().BeSameAs(logisticsLocations);
        }



        [Test]
        public async Task Service_Returns_True_When_Calling_RemoveEstablishmentFromPartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocation = new LogisticsLocationDTO
            {
                TradePartyId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            _mockApiIntegration.Setup(x => x.RemoveEstablishmentAsync(new Guid())).Verifiable();

            // Act
            var returnedValue = await _establishmentService.RemoveEstablishmentAsync(new Guid());

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(true);
        }

        [Test]
        public async Task Service_Returns_True_When_Calling_UpdateEstablishmentDetails()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocationDto = new LogisticsLocationDTO
            {
                Id = Guid.NewGuid(),
                Name = "testname",
                NI_GBFlag = "GB",
            };

            _mockApiIntegration.Setup(x => x.UpdateEstablishmentAsync(logisticsLocationDto)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateEstablishmentAsync(logisticsLocationDto)).Returns(Task.FromResult(true)!);

            // Act
            var returnedValue = await _establishmentService.UpdateEstablishmentDetailsAsync(logisticsLocationDto);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(true);
        }
    }
}
