﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
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
        public async Task Service_Follows_Correct_Route_When_Calling_CreateTradePartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var logisticsLocationDto = new LogisticsLocationDTO
            {
                Name = "Logistics location"
            };

            _mockApiIntegration.Setup(x => x.CreateEstablishmentAsync(logisticsLocationDto)).Verifiable();
            _mockApiIntegration.Setup(x => x.CreateEstablishmentAsync(logisticsLocationDto)).Returns(Task.FromResult(expectedGuid as Guid?));

            // Act
            var returnedValue = await _establishmentService.CreateEstablishmentAndAddToPartyAsync(expectedGuid, logisticsLocationDto);

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

            var logisticsLocationDto = new List<LogisticsLocationDetailsDTO>();

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
        public async Task Service_Follows_Correct_Route_When_Calling_AddEstablishmentToPartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = Guid.NewGuid(),
                LogisticsLocationId = Guid.NewGuid()
            };

            _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(logisticsLocations)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(logisticsLocations).Result).Returns(logisticsLocations.TradePartyId);

            // Act
            var returnedValue = await _establishmentService.AddEstablishmentToPartyAsync(logisticsLocations);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(logisticsLocations.TradePartyId);
        }

        [Test]
        public async Task Service_Returns_True_When_Calling_RemoveEstablishmentFromPartyAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = Guid.NewGuid(),
                LogisticsLocationId = Guid.NewGuid()
            };

            _mockApiIntegration.Setup(x => x.RemoveEstablishmentFromPartyAsync(new Guid(), new Guid())).Verifiable();

            // Act
            var returnedValue = await _establishmentService.RemoveEstablishmentFromPartyAsync(new Guid(), new Guid());

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(true);
        }

        [Test]
        public async Task Service_Returns_False_When_Calling_IsFirstTradePartyForEstablishment()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = Guid.NewGuid(),
                LogisticsLocationId = Guid.NewGuid()
            };

            var list = new List<LogisticsLocationBusinessRelationshipDTO> { logisticsLocations };

            _mockApiIntegration.Setup(x => x.GetAllRelationsForEstablishmentAsync(new Guid())).Verifiable();
            _mockApiIntegration.Setup(x => x.GetAllRelationsForEstablishmentAsync(new Guid())).Returns(Task.FromResult(list)!);

            // Act
            var returnedValue = await _establishmentService.IsFirstTradePartyForEstablishment(new Guid(), new Guid());

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(false);
        }

        [Test]
        public async Task Service_Returns_True_When_Calling_IsFirstTradePartyForEstablishment()
        {
            // Arrange
            var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new LogisticsLocationBusinessRelationshipDTO
            {
                TradePartyId = guid,
                LogisticsLocationId = guid,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var list = new List<LogisticsLocationBusinessRelationshipDTO> { logisticsLocations };

            _mockApiIntegration.Setup(x => x.GetAllRelationsForEstablishmentAsync(guid)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetAllRelationsForEstablishmentAsync(guid)).Returns(Task.FromResult(list)!);

            // Act
            var returnedValue = await _establishmentService.IsFirstTradePartyForEstablishment(guid, guid);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(true);
        }
    }
}
