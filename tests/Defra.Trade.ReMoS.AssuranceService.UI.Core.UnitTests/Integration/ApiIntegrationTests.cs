using Defra.Trade.Address.V1.ApiClient.Model;
using Defra.Trade.Common.Security.Authentication.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Configuration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Integration;

internal class ApiIntegrationTests
{
    private IApiIntegration? _apiIntegration;
    protected Mock<IHttpClientFactory> _mockHttpClientFactory = new();
    protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();
    private readonly Mock<IAuthenticationService> _mockAuthenticationService = new();

    [SetUp]
    public void Setup() 
    {
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);
        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);
    }

    [Test]
    public async Task Integration_Returns_TradeParties_When_Calling_GetAllTradePartiesAsync()
    {
        // Arrange
        var tradeParties = new List<TradePartyDto> {  new TradePartyDto(), new TradePartyDto() };

        var jsonString = JsonConvert.SerializeObject(tradeParties);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetAllTradePartiesAsync();

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Count.Should().Be(tradeParties.Count);
    }

    [Test]
    public async Task Integration_Returns_TradePartyDTO_When_Calling_GetTradePartyByIdAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(tradeParty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetTradePartyByIdAsync(Guid.Empty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
    }

    [Test]
    public async Task Integration_Returns_TradePartyDTO_When_Calling_GetTradePartyByOrgIdAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(tradeParty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetTradePartyByOrgIdAsync(Guid.Empty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
    }

    [Test]
    public async Task Integration_Returns_NotFound_TradePartyDTO_When_Calling_GetTradePartyByOrgIdAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(tradeParty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetTradePartyByOrgIdAsync(Guid.Empty);

        // Assert
        _mockHttpClientFactory.Verify();
        Assert.AreEqual(null, returnedValue);
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_AddTradePartyAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.AddTradePartyAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    public async Task Integration_Returns_200_When_Calling_UpdateTradePartyAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateTradePartyAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }


    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_AddTradePartyAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.AddTradePartyAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateTradePartyAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_UpdateTradePartyAddressAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateTradePartyAddressAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateTradePartyAddressAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyAddressAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_AddAddressToPartyAsync()
    {
        // Arrange
        var tradeAddress = new TradeAddressDto
        {
            TradeCountry = "United Kingdom",
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.AddAddressToPartyAsync(guid, tradeAddress);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Returns_BadHttpRequestException_When_Calling_AddAddressToPartyAsync_WithBadData()
    {
        // Arrange
        var tradeAddress = new TradeAddressDto
        {
            TradeCountry = "United Kingdom",
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.Empty;

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act + Assert
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.AddAddressToPartyAsync(guid, tradeAddress));
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_UpdateTradePartyContactAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateTradePartyContactAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_UpdateTradePartyContactSelfServeAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateTradePartyContactSelfServeAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_UpdateTradePartyAuthRepSelfServeAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateTradePartyAuthRepSelfServeAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }


    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateTradePartyContactAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyContactAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateTradePartyContactSelfServeAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyContactSelfServeAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateTradePartyAuthRepSelfServeAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            Id = Guid.Empty,
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyAuthRepSelfServeAsync(tradeParty));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_AddEstablishmentToPartyAsync()
    {
        // Arrange
        var logisticsLocationDto = new LogisticsLocationDto
        {
            Name = "Trade party Ltd"
        };
        var partyId = Guid.NewGuid();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(guid);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.AddEstablishmentToPartyAsync(partyId,logisticsLocationDto);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().Be(guid);
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "Establishment already exists")]
    public async Task Integration_Returns_Already_Exists_Calling_AddEstablishmentToPartyAsync()
    {
        // Arrange
        var logisticsLocationDto = new LogisticsLocationDto
        {
            Name = "Trade party Ltd"
        };
        var partyId = Guid.NewGuid();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();
        var errMessage = "\"Establishment already exists\"";

        var jsonString = JsonConvert.SerializeObject(errMessage);
        var httpContent = new StringContent(errMessage, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.AddEstablishmentToPartyAsync(partyId, logisticsLocationDto));

        // Assert
        _mockHttpClientFactory.Verify();
    }


    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_CreateEstablishmentAsync()
    {
        var logisticsLocationDto = new LogisticsLocationDto
        {
            Name = "Trade party Ltd"
        };
        var partyId = Guid.NewGuid();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(Guid.Empty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.AddEstablishmentToPartyAsync(partyId,logisticsLocationDto));

        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    public async Task Integration_Returns_Guid_When_Calling_GetEstablishmentByIdAsync()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var location = new LogisticsLocationDto();
        var jsonString = JsonConvert.SerializeObject(location);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetEstablishmentByIdAsync(guid);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Should().BeOfType<LogisticsLocationDto>();
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException), "Work in Progress")]
    public async Task Integration_Returns_LogisticsLocations_When_Calling_GetEstablishmentsForTradePartyAsync()
    {
        // Arrange
        var logisticsLocations = new List<LogisticsLocationDto>
        {
            new LogisticsLocationDto()
            {

                Name = "Test 2",
                Id = Guid.NewGuid(),
                TradePartyId = Guid.NewGuid(),
            }
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(logisticsLocations, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetEstablishmentsForTradePartyAsync(It.IsAny<Guid>());

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Count.Should().Be(1);
        returnedValue[0].Name.Should().Be(logisticsLocations[0].Name);
    }

    [Test]
    public async Task Integration_Returns_LogisticsLocations_When_Calling_GetEstablishmentsByPostcodeAsync()
    {
        // Arrange
        var logisticsLocations = new List<LogisticsLocationDto>
        {
            new LogisticsLocationDto()
            {
                Name = "Test 2",
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto()
                {
                    LineOne = "line 1",
                    CityName = "city",
                    PostCode = "TES1",
                }
            }
        };
        var postcode = "TES1";
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(logisticsLocations, new JsonSerializerSettings { ContractResolver= new CamelCasePropertyNamesContractResolver() });
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.GetEstablishmentsByPostcodeAsync(postcode);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Count.Should().Be(1);
        returnedValue[0].Name.Should().Be(logisticsLocations[0].Name);
        returnedValue[0].Address!.LineOne.Should().Be(logisticsLocations[0].Address!.LineOne);
        returnedValue[0].Address!.CityName.Should().Be(logisticsLocations[0].Address!.CityName);
        returnedValue[0].Address!.PostCode.Should().Be(logisticsLocations[0].Address!.PostCode);
    }

    [Test]
    public async Task Integration_Verify_When_Calling_RemoveEstablishmentFromPartyAsync()
    {
        // Arrange
        var logisticsLocations = new List<LogisticsLocationDto>
        {
            new LogisticsLocationDto()
            {
                Name = "Test 2",
                Id = Guid.NewGuid(),
                Address = new TradeAddressDto()
                {
                    LineOne = "line 1",
                    CityName = "city",
                    PostCode = "TES1",
                }
            }
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(logisticsLocations, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await _apiIntegration.RemoveEstablishmentAsync(new Guid());

        // Assert
        _mockHttpClientFactory.Verify();
    }       

    [Test]
    public async Task Integration_Returns_TradePartyDTO_When_Calling_UpdateAuthorisedSignatoryAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto
        {
            PartyName = "Trade party Ltd",
            NatureOfBusiness = "Wholesale Hamster Supplies",
            CountryName = "United Kingdom"
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(tradeParty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateAuthorisedSignatoryAsync(tradeParty);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
    }

    [Test]
    public async Task Integration_Returns_True_When_Calling_UpdateEstablishment()
    {
        // Arrange
        var logisticsLocationDto = new LogisticsLocationDto
        {
            Id = Guid.NewGuid(),
            Name = "testname",
            NI_GBFlag = "GB",
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(logisticsLocationDto);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateEstablishmentAsync(logisticsLocationDto);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue.Should().Be(true);
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Throws_BadHttpRequestException_When_Calling_With_Bad_Data_UpdateEstablishmentAsync()
    {
        // Arrange
        var logisticsLocationDto = new LogisticsLocationDto
        {
        };

        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(logisticsLocationDto);

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateEstablishmentAsync(logisticsLocationDto));


        // Assert
        _mockHttpClientFactory.Verify();
    }

    [Test]
    public async Task GetTradeAddresApiByPostcodeAsync_ReturnsAddress()
    {
        // arrange
        var postcode = "TES1";
        var addressDto = new AddressDto("123", null, null, null, null, null, postcode);
        var addressesDto = new List<AddressDto>()
        {
            addressDto
        };

        var jsonString = JsonConvert.SerializeObject(addressesDto, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        // Act
        var returnedValue = await _apiIntegration!.GetTradeAddresApiByPostcodeAsync(postcode);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue!.Count.Should().Be(1);
        returnedValue[0].Uprn.Should().Be("123");
        returnedValue[0].Postcode.Should().Be(postcode);
    }

    [Test]
    public async Task GetLogisticsLocationByUprnAsync_ReturnsLocation()
    {
        // Arrange
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
        var jsonString = JsonConvert.SerializeObject(logisticsLocation, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = httpContent,
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        // Act
        var returnedValue = await _apiIntegration!.GetLogisticsLocationByUprnAsync(uprn);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue.Should().BeOfType<LogisticsLocationDto>();
    }

    [Test]
    [ExpectedException(typeof(BadHttpRequestException), "null return from API")]
    public async Task Integration_Returns_BadHttpException_When_Calling_GetTradePartyByOrgIdAsync()
    {
        // Arrange
        var tradeParty = new TradePartyDto();
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var jsonString = JsonConvert.SerializeObject(tradeParty);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act

        // Assert
        await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.GetTradePartyByOrgIdAsync(Guid.Empty));
    }

    [Test]
    public async Task Integration_Returns_True_When_Calling_UpdateEstablishmentSelfServe()
    {
        // Arrange
        var logisticsLocationDto = new LogisticsLocationDto
        {
            Id = Guid.NewGuid(),
            Name = "testname",
            NI_GBFlag = "GB",
        };
        var appConfigurationSettings = new AppConfigurationService
        {
            SubscriptionKey = "testkey"
        };
        IOptions<AppConfigurationService> appConfigurationSettingsOptions = Options.Create(appConfigurationSettings);

        var guid = Guid.NewGuid();

        var jsonString = JsonConvert.SerializeObject(logisticsLocationDto);
        var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        var expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NoContent,
            Content = httpContent
        };

        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

        _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object, appConfigurationSettingsOptions, _mockAuthenticationService.Object);

        // Act
        var returnedValue = await _apiIntegration.UpdateEstablishmentSelfServeAsync(logisticsLocationDto);

        // Assert
        _mockHttpClientFactory.Verify();
        returnedValue.Should().Be(true);
    }
}
