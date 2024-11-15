﻿using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Helpers;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Moq;
using NUnit.Framework;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Services;

[TestFixture]
public class EstablishmentServiceTests
{
    private IEstablishmentService? _establishmentService;
    protected Mock<IApiIntegration> _mockApiIntegration = new();

    [Test]
    public async Task Service_Follows_Correct_Route_When_Calling_CreateEstablishmentForTradePartyAsync()
    {
        // Arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

        var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

        var logisticsLocationDto = new LogisticsLocationDto
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

        var logisticsLocationDto = new PagedList<LogisticsLocationDto>();

        _mockApiIntegration.Setup(x => x.GetEstablishmentsForTradePartyAsync(expectedGuid, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();
        _mockApiIntegration
            .Setup(x => x.GetEstablishmentsForTradePartyAsync(expectedGuid, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.FromResult(logisticsLocationDto)!);

        // Act
        var returnedValue = await _establishmentService.GetEstablishmentsForTradePartyAsync(expectedGuid, false, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());

        // Assert
        _mockApiIntegration.Verify();
        returnedValue.Should().Be(logisticsLocationDto);
    }

    [Test]
    public async Task Service_Returns_LogisticsLocationDTO_When_Calling_GetEstablishmentByIdAsync()
    {
        // Arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);

        var expectedGuid = Guid.Parse("c16eb7a7-2949-4880-b5d7-0405f4f7d188");

        var logisticsLocationDto = new LogisticsLocationDto
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

        var logisticsLocations = new List<LogisticsLocationDto>
        {
            new LogisticsLocationDto()
            {
                TradeAddressId = Guid.NewGuid()
            }
        };
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

        var logisticsLocation = new LogisticsLocationDto
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

        var logisticsLocationDto = new LogisticsLocationDto
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

    [Test]
    public async Task GetTradeAddressApiByPostcodeAsync_ReturnsTrue()
    {
        // Arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        var postcode = "TES1";
        var addressDto = new AddressDto("1234", null, null, null, null, 0, null, 0, null, postcode, null, null, null, null, "123", 0, 0);
        var addressesDto = new List<AddressDto>()
        {
            addressDto
        };

        _mockApiIntegration.Setup(x => x.GetTradeAddresApiByPostcodeAsync(postcode)).Verifiable();
        _mockApiIntegration.Setup(x => x.GetTradeAddresApiByPostcodeAsync(postcode)).Returns(Task.FromResult(addressesDto)!);

        // Act
        var returnedValue = await _establishmentService.GetTradeAddressApiByPostcodeAsync(postcode);

        // Assert
        _mockApiIntegration.Verify();
        returnedValue.Should().BeEquivalentTo(addressesDto);
    }

    [Test]
    public async Task GetLogisticsLocationByUprnAsync_ReturnsTrue()
    {
        // Arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        var uprn = "123";
        var logisticsLocation = new LogisticsLocationDto()
        {
            Name = "Test 2",
            Id = Guid.NewGuid(),
            Address = new TradeAddressDto()
            {
                LineOne = "line 1",
                CityName = "city",
                PostCode = "TES1",
            }
        };

        _mockApiIntegration.Setup(x => x.GetLogisticsLocationByUprnAsync(uprn)).Verifiable();
        _mockApiIntegration.Setup(x => x.GetLogisticsLocationByUprnAsync(uprn)).Returns(Task.FromResult(logisticsLocation)!);

        // Act
        var returnedValue = await _establishmentService.GetLogisticsLocationByUprnAsync(uprn);

        // Assert
        _mockApiIntegration.Verify();
        returnedValue.Should().BeEquivalentTo(logisticsLocation);
    }

    [Test]
    public async Task UpdateEstablishmentDetailsSelfServeAsync_ReturnsTrue()
    {
        // Arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        var establishment = new LogisticsLocationDto();
        _mockApiIntegration
            .Setup(x => x.UpdateEstablishmentSelfServeAsync(It.IsAny<LogisticsLocationDto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _establishmentService!.UpdateEstablishmentDetailsSelfServeAsync(establishment);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task SaveEstablishmentDetails_Create()
    {
        // arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        Guid? expected = Guid.NewGuid();
        var tradePartyId = Guid.NewGuid();
        var establishmentDto = new LogisticsLocationDto()
        {
            Name = "Test name",
            Address = new TradeAddressDto()
            {
                LineOne = "Line one",
                LineTwo = "Line two",
                CityName = "City",
                County = "Berkshire",
                PostCode = "TES1"
            },
            NI_GBFlag = "GB"
        };
        _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>())).ReturnsAsync(expected);

        // act
        var result = await _establishmentService!.SaveEstablishmentDetails(null, tradePartyId, establishmentDto, "GB", string.Empty);

        // assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task SaveEstablishmentDetails_EstablishmentFromApi_Updated_With_Given_Dto()
    {
        // arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        var establishmentId = Guid.NewGuid();
        var tradePartyId = Guid.NewGuid();
        var establishmentFromApi = new LogisticsLocationDto()
        {
            Id = establishmentId,
            Name = "Test name",
            Address = new TradeAddressDto()
            {
                LineOne = "Line one",
                LineTwo = "Line two",
                CityName = "City",
                County = "Berkshire",
                PostCode = "TES1"
            },
            ApprovalStatus = LogisticsLocationApprovalStatus.Draft,
        };
        var establishmentDto = new LogisticsLocationDto()
        {
            Name = "New name",
            Address = new TradeAddressDto()
            {
                LineOne = "New Line one",
                LineTwo = "Line two",
                CityName = "City",
                County = "Berkshire",
                PostCode = "TES1"
            }
        };
        _mockApiIntegration.Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(establishmentFromApi);
        _mockApiIntegration.Setup(x => x.AddEstablishmentToPartyAsync(It.IsAny<Guid>(), It.IsAny<LogisticsLocationDto>())).ReturnsAsync(establishmentId);

        // act
        var result = await _establishmentService!.SaveEstablishmentDetails(establishmentId, tradePartyId, establishmentDto, "GB", null);

        // assert
        //result.Should().NotBe(null);
        result.GetType().Should().Be(typeof(Guid));
    }

    [TestCase(LogisticsLocationApprovalStatus.Draft, true)]
    [TestCase(LogisticsLocationApprovalStatus.Approved, false)]
    public async Task IsEstablishmentDraft_Returns(LogisticsLocationApprovalStatus approvalStatus, bool isEstablishment)
    {
        // arrange
        _establishmentService = new EstablishmentService(_mockApiIntegration.Object);
        var establishmentId = Guid.NewGuid();
        var establishmentFromApi = new LogisticsLocationDto()
        {
            Id = establishmentId,
            Name = "Test name",
            Address = new TradeAddressDto()
            {
                LineOne = "Line one",
                LineTwo = "Line two",
                CityName = "City",
                County = "Berkshire",
                PostCode = "TES1"
            },
            ApprovalStatus = approvalStatus,
        };
        _mockApiIntegration.Setup(x => x.GetEstablishmentByIdAsync(It.IsAny<Guid>())).ReturnsAsync(establishmentFromApi);

        // act
        var result = await _establishmentService.IsEstablishmentDraft(establishmentId);

        // assert
        result.Should().Be(isEstablishment);
    }
}