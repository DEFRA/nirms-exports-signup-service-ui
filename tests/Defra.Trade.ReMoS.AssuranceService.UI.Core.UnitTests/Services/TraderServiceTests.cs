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
using System.Runtime.Intrinsics.X86;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    [TestFixture]
    internal class TraderServiceTests
    {
        private ITraderService? _traderService;
        protected Mock<IAPIIntegration> _mockApiIntegration = new();

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_CreateTradePartyAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradePartyDTO = new TradePartyDTO
            {
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.AddTradePartyAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddTradePartyAsync(tradePartyDTO)).Returns(Task.FromResult(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188")));

            // Act
            var returnedValue = await _traderService.CreateTradePartyAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            expectedGuid.Should().Be(returnedValue);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var tradePartyDTO = new TradePartyDTO
            {
                Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.UpdateTradePartyAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateTradePartyAsync(tradePartyDTO)).Returns(Task.FromResult(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188")));

            // Act
            var returnedValue = await _traderService.UpdateTradePartyAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            tradePartyDTO.Id.Should().Be(returnedValue);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_GetTradePartyByIdAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var tradePartyDTO = new TradePartyDTO
            {
                Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup( x => x.GetTradePartyByIdAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"))).Verifiable();
            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"))).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var returnedValue = await _traderService.GetTradePartyByIdAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"));

            // Assert
            _mockApiIntegration.Verify();
            tradePartyDTO.Should().Be(returnedValue);
        }

        [Test]
        public async Task Service_Creates_New_TradePartyDTO_When_Calling_With_Empty_Id_GetTradePartyByIdAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var emptyTradePartyDTO = new TradePartyDTO
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Empty)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Empty)).Returns(Task.FromResult(new TradePartyDTO())!);

            // Act
            var returnedValue = await _traderService.GetTradePartyByIdAsync(Guid.Empty);

            // Assert
            _mockApiIntegration.Verify();
            emptyTradePartyDTO.Id.Should().Be(returnedValue!.Id);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyAddressAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradePartyDTO = new TradePartyDTO
            {
                Id = guid,
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.UpdateTradePartyAddressAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateTradePartyAddressAsync(tradePartyDTO)).Returns(Task.FromResult(guid)!);

            // Act
            var returnedValue = await _traderService.UpdateTradePartyAddressAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(guid);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyContactAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradePartyDTO = new TradePartyDTO
            {
                Id = guid,
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.UpdateTradePartyContactAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateTradePartyContactAsync(tradePartyDTO)).Returns(Task.FromResult(guid)!);

            // Act
            var returnedValue = await _traderService.UpdateTradePartyContactAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(guid);
        }
    }
}
