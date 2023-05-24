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
        public async Task Service_Follows_Correct_Route_When_Calling_GetLogisticsLocationByPostcodeAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

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

            var postcode = "TES1";

            _mockApiIntegration.Setup(x => x.GetLogisticsLocationByPostcodeAsync(postcode)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetLogisticsLocationByPostcodeAsync(postcode).Result).Returns(logisticsLocations);

            // Act
            var returnedValue = await _establishmentService.GetLogisticsLocationByPostcodeAsync(postcode);

            // Assert
            _mockApiIntegration.Verify();
            Assert.AreEqual(logisticsLocations, returnedValue);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_AddLogisticsLocationRelationshipAsync()
        {
            // Arrange
            _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

            var logisticsLocations = new LogisticsLocationRelationshipDTO
            {
                TraderId = Guid.NewGuid(),
                EstablishmentId = Guid.NewGuid()
            };

            _mockApiIntegration.Setup(x => x.AddLogisticsLocationRelationship(logisticsLocations)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddLogisticsLocationRelationship(logisticsLocations).Result).Returns(logisticsLocations.EstablishmentId);

            // Act
            var returnedValue = await _establishmentService.AddLogisticsLocationRelationshipAsync(logisticsLocations);

            // Assert
            _mockApiIntegration.Verify();
            Assert.AreEqual(logisticsLocations.EstablishmentId, returnedValue);
        }
    }
}
