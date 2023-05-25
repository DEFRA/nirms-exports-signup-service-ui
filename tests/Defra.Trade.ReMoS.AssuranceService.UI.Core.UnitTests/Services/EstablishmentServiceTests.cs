using Defra.Trade.ReMoS.AssuranceService.UI.Core.TagHelpers;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    [TestFixture]
    public class EstablishmentServiceTests
    {
        private IEstablishmentService? _establishmentService;
        protected Mock<IAPIIntegration> _mockApiIntegration = new();

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
            Assert.AreEqual(logisticsLocations, returnedValue);
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
            Assert.AreEqual(logisticsLocations.TradePartyId, returnedValue);
        }
    }
}
