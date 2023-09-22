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
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Enums;
using System.Security.Claims;
using System.Text.Json;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services
{
    [TestFixture]
    internal class TraderServiceTests
    {
        private ITraderService? _traderService;
        protected Mock<IApiIntegration> _mockApiIntegration = new();

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_CreateTradePartyAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var TradePartyDTO = new TradePartyDto
            {
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.AddTradePartyAsync(TradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddTradePartyAsync(TradePartyDTO)).Returns(Task.FromResult(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188")));

            // Act
            var returnedValue = await _traderService.CreateTradePartyAsync(TradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            expectedGuid.Should().Be(returnedValue);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);

            var tradePartyDTO = new TradePartyDto
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

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188"))).Verifiable();
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

            var emptyTradePartyDTO = new TradePartyDto
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000")
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Empty)).Verifiable();
            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(Guid.Empty)).Returns(Task.FromResult(new TradePartyDto())!);

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

            var tradePartyDTO = new TradePartyDto
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
        public async Task Service_ReturnsGuid_When_Calling_AddTradePartyAddressAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var partyId = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradeAddressDto = new TradeAddressDto
            {
                TradeCountry = "GB",
            };

            _mockApiIntegration.Setup(x => x.AddAddressToPartyAsync(partyId, tradeAddressDto)).Verifiable();
            _mockApiIntegration.Setup(x => x.AddAddressToPartyAsync(partyId, tradeAddressDto)).Returns(Task.FromResult(partyId)!);

            // Act
            var returnedValue = await _traderService.AddTradePartyAddressAsync(partyId, tradeAddressDto);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(partyId);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyContactAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradePartyDTO = new TradePartyDto
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

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateAuthorisedSignatoryAsync()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var guid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

            var tradePartyDTO = new TradePartyDto
            {
                Id = guid,
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            _mockApiIntegration.Setup(x => x.UpdateAuthorisedSignatoryAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateAuthorisedSignatoryAsync(tradePartyDTO)).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var returnedValue = await _traderService.UpdateAuthorisedSignatoryAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(tradePartyDTO);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_New_WhenPartyIsNull()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            //var tradePartyDto = new TradePartyDTO { Id = Guid.NewGuid() };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TradePartyDto)null!);
            var expectedResult = ((TradePartyDto)null!, TradePartySignupStatus.New);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_New_WhenAddressIsNull()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto { Id = Guid.NewGuid() };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.New);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_Complete_When_TAndCSigned()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto { Id = Guid.NewGuid(), Address = new TradeAddressDto { TradeCountry = "GB" }, TermsAndConditionsSignedDate = DateTime.Now };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.Complete);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_Null_When_Rejected()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto { Id = Guid.NewGuid(), 
                Address = new TradeAddressDto { TradeCountry = "GB" }, 
                TermsAndConditionsSignedDate = DateTime.Now,
                ApprovalStatus = TradePartyApprovalStatus.Rejected
            };

            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be((null, TradePartySignupStatus.New));
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgress_When_CountryAndFboAndRegulationsFilled()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto { TradeCountry = "GB" },
                FboNumber = "1234",
                RegulationsConfirmed = true,
            };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.InProgress);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgressEligibilityCountry_When_Country_Is_Null()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto { Id = Guid.NewGuid() },
            };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.InProgressEligibilityCountry);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgressEligibilityFboNumber_When_FboNumber_Is_Null()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto { Id = Guid.NewGuid(), TradeCountry = "GB" },
            };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.InProgressEligibilityFboNumber);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgressEligibilityRegulations_When_Regulatons_Not_Confirmed()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto { Id = Guid.NewGuid(), TradeCountry = "GB" },
                FboNumber = "1234",
            };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(tradePartyDto);
            var expectedResult = (tradePartyDto, TradePartySignupStatus.InProgressEligibilityRegulations);

            // Act
            var returnedValue = await _traderService!.GetDefraOrgBusinessSignupStatus(orgId);

            // Assert
            returnedValue.Should().Be(expectedResult);
        }

        [Test]
        public async Task ValidateOrgId_Returns_True()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);
            var claims = new List<Claim>
            {
                new Claim("userEnrolledOrganisations", "[{\"organisationId\":\"152691a1-6d8b-e911-a96f-000d3a29b5de\",\"practiceName\":\"ABC ACCOUNTANCY & MARKETING SERVICES LTD\"}]")
            };

            var guid = Guid.Parse("4e2df7aa-8141-49b7-ad54-44e15ba24bec");

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.Parse("152691a1-6d8b-e911-a96f-000d3a29b5de")
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var result = await _traderService!.ValidateOrgId(claims, Guid.NewGuid());

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ValidateOrgId_Returns_False()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);
            var claims = new List<Claim>
            {
                new Claim("userEnrolledOrganisations", "[{\"organisationId\":\"152691a1-6d8b-e911-a96f-000d3a29b5de\",\"practiceName\":\"ABC ACCOUNTANCY & MARKETING SERVICES LTD\"}]")
            };

            var guid = Guid.Parse("4e2df7aa-8141-49b7-ad54-44e15ba24bec");

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid()
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var result = await _traderService!.ValidateOrgId(claims, Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsTradePartySignedUp_Returns_True()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);

            var guid = Guid.Parse("4e2df7aa-8141-49b7-ad54-44e15ba24bec");

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid(),
                SignUpRequestSubmittedBy = Guid.NewGuid()                
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var result = await _traderService!.IsTradePartySignedUp(Guid.NewGuid());

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsTradePartySignedUp_Returns_False()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);

            var guid = Guid.Parse("4e2df7aa-8141-49b7-ad54-44e15ba24bec");

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid(),
                SignUpRequestSubmittedBy = Guid.Empty
            };

            _mockApiIntegration.Setup(x => x.GetTradePartyByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var result = await _traderService!.IsTradePartySignedUp(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }

    }
}