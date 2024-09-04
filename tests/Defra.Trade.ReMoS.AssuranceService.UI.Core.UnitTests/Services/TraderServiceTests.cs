using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

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
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyContactSelfServeAsync()
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

            _mockApiIntegration.Setup(x => x.UpdateTradePartyContactSelfServeAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateTradePartyContactSelfServeAsync(tradePartyDTO)).Returns(Task.FromResult(guid)!);

            // Act
            var returnedValue = await _traderService.UpdateTradePartyContactSelfServeAsync(tradePartyDTO);

            // Assert
            _mockApiIntegration.Verify();
            returnedValue.Should().Be(guid);
        }

        [Test]
        public async Task Service_Follows_Correct_Route_When_Calling_UpdateTradePartyAuthRepSelfServeAsync()
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

            _mockApiIntegration.Setup(x => x.UpdateTradePartyAuthRepSelfServeAsync(tradePartyDTO)).Verifiable();
            _mockApiIntegration.Setup(x => x.UpdateTradePartyAuthRepSelfServeAsync(tradePartyDTO)).Returns(Task.FromResult(guid)!);

            // Act
            var returnedValue = await _traderService.UpdateAuthRepSelfServeAsync(tradePartyDTO);

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
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgressEligibilityRegulations_WhenAddressIsNull_And_RegulationsNotConfirmed()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
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
        public async Task GetDefraOrgBusinessSignupStatus_Returns_InProgressEligibilityCountry_WhenAddressIsNull_And_RegulationsConfirmed()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                RegulationsConfirmed = true,
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
        public async Task GetDefraOrgBusinessSignupStatus_Returns_Complete_When_TAndCSigned()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                RegulationsConfirmed = true,
                Address = new TradeAddressDto { TradeCountry = "GB" },
                TermsAndConditionsSignedDate = DateTime.Now
            };
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
            var tradePartyDto = new TradePartyDto
            {
                Id = Guid.NewGuid(),
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
                FboPhrOption = "fbo",
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
                RegulationsConfirmed = true,
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
                FboPhrOption = "fbo"
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
        public void ValidateOrgId_Returns_True()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);
            var claims = new List<Claim>
            {
                new Claim("userEnrolledOrganisations", "[{\"organisationId\":\"152691a1-6d8b-e911-a96f-000d3a29b5de\",\"practiceName\":\"ABC ACCOUNTANCY & MARKETING SERVICES LTD\"}]")
            };

            var guid = Guid.Parse("152691a1-6d8b-e911-a96f-000d3a29b5de");

            // Act
            var result = _traderService!.ValidateOrgId(claims, guid);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateOrgId_Returns_False()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);
            var claims = new List<Claim>
            {
                new Claim("userEnrolledOrganisations", "[{\"organisationId\":\"152691a1-6d8b-e911-a96f-000d3a29b5de\",\"practiceName\":\"ABC ACCOUNTANCY & MARKETING SERVICES LTD\"}]")
            };

            var guid = Guid.Parse("4e2df7aa-8141-49b7-ad54-44e15ba24bec");

            // Act
            var result = _traderService!.ValidateOrgId(claims, guid);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTradePartySignedUp_Returns_True()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid(),
                SignUpRequestSubmittedBy = Guid.NewGuid()
            };

            // Act
            var result = _traderService!.IsTradePartySignedUp(tradePartyDTO);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsTradePartySignedUp_Returns_False()
        {
            // Assert
            _traderService = new TraderService(_mockApiIntegration.Object);

            var tradePartyDTO = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid(),
                SignUpRequestSubmittedBy = Guid.Empty
            };

            // Act
            var result = _traderService!.IsTradePartySignedUp(tradePartyDTO);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetDefraOrgApprovalStatus_Throws_Exception_For_EmptyGuid()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            _mockApiIntegration.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(It.IsAny<TradePartyDto>());

            // Act & Assert
            Assert.ThrowsAsync(typeof(ArgumentNullException), async () => await _traderService!.GetDefraOrgApprovalStatus(Guid.Empty));
        }

        [Test]
        public async Task GetDefraOrgApprovalStatus_Returns_NotSIgnedUp_For_New_Org()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            TradePartyDto tradePartyDTO = null!;
            _mockApiIntegration.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(tradePartyDTO)!);

            // Act
            var result = await _traderService!.GetDefraOrgApprovalStatus(Guid.NewGuid());

            // Assert
            Assert.That(TradePartyApprovalStatus.NotSignedUp, Is.EqualTo(result));
        }

        [Test]
        public async Task GetDefraOrgApprovalStatus_Returns_Correct_ApprovalStatus_If_Tradeparty_Exists()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            TradePartyDto tradePartyDTO = new TradePartyDto()
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = Guid.NewGuid(),
                SignUpRequestSubmittedBy = Guid.Empty,
                ApprovalStatus = TradePartyApprovalStatus.SignupStarted
            };
            _mockApiIntegration.Setup(x => x.GetTradePartyByOrgIdAsync(It.IsAny<Guid>())).ReturnsAsync(tradePartyDTO);

            // Act
            var result = await _traderService!.GetDefraOrgApprovalStatus(tradePartyDTO.Id);

            // Assert
            Assert.That(result, Is.EqualTo(TradePartyApprovalStatus.SignupStarted));
        }

        [Test]
        public async Task GetTradePartyByOrgIdAsync_WhenOrgIdIsEmpty_ReturnNewTradePartyDto()
        {
            // arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.Empty;

            // act
            var result = await _traderService.GetTradePartyByOrgIdAsync(orgId);

            // assert
            result!.Id.Should().Be(Guid.Empty);
            result!.OrgId.Should().Be(Guid.Empty);
        }

        [Test]
        public async Task GetTradePartyByOrgIdAsync_WhenOrgIdIsValid_ReturnValidTradePartyDto()
        {
            // arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var orgId = Guid.NewGuid();
            TradePartyDto tradePartyDTO = new TradePartyDto()
            {
                Id = Guid.NewGuid(),
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom",
                OrgId = orgId,
                SignUpRequestSubmittedBy = Guid.Empty,
                ApprovalStatus = TradePartyApprovalStatus.SignupStarted
            };
            _mockApiIntegration.Setup(x => x.GetTradePartyByOrgIdAsync(orgId)).ReturnsAsync(tradePartyDTO);

            // act
            var result = await _traderService.GetTradePartyByOrgIdAsync(orgId);

            // assert
            Assert.That(tradePartyDTO, Is.EqualTo(result));
        }

        [Test]
        public async Task GetBusinessNameAsync_Returns_BusinessName()
        {
            // Arrange
            _traderService = new TraderService(_mockApiIntegration.Object);
            var expected = new TradePartyDto
            {
                Id = Guid.NewGuid(),
                PracticeName = "TestName",
            };
            _mockApiIntegration
                .Setup(x => x.GetTradePartyByIdAsync(expected.Id))
                .ReturnsAsync(expected);

            // Act
            var result = await _traderService.GetBusinessNameAsync(expected.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expected.PracticeName);
        }
    }
}