using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    internal class EstablishmentServiceTests
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
        [ExpectedException(typeof(NotImplementedException), "Work in Progress")]
        public async Task Services_Throws_NotImplementedException_When_Calling_GetEstablishmentsForTradePartyAsync()
        {
            //Arrange
            //TODO: Remove once method is completed
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            //Act, Assert
            await Microsoft.VisualStudio.TestTools.UnitTesting.Assert
                .ThrowsExceptionAsync<NotImplementedException>(async () => await _establishmentService!.GetEstablishmentsForTradePartyAsync(new Guid()));

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
    }
}
