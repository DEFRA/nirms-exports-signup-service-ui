using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Defra.Trade.ReMoS.AssuranceService.UI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Integration
{
    internal class ApiIntegrationTests
    {
        private IAPIIntegration? _apiIntegration;
        protected Mock<IHttpClientFactory> _mockHttpClientFactory = new();
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        [Test]
        public async Task Integration_Returns_TradeParties_When_Calling_GetAllTradePartiesAsync()
        {
            // Arrange
            var tradeParties = new List<TradePartyDTO> {  new TradePartyDTO(), new TradePartyDTO() };

            var jsonString = JsonConvert.SerializeObject(tradeParties);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);
            
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            var returnedValue = await _apiIntegration.GetAllTradePartiesAsync();

            // Assert
            _mockHttpClientFactory.Verify();
            returnedValue!.Count().Should().Be(tradeParties.Count());
        }

        [Test]
        public async Task Integration_Returns_TradePartyDTO_When_Calling_GetTradePartyByIdAsync()
        {
            // Arrange
            var tradeParty = new TradePartyDTO();

            var jsonString = JsonConvert.SerializeObject(tradeParty);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            var returnedValue = await _apiIntegration.GetTradePartyByIdAsync(Guid.Empty);

            // Assert
            _mockHttpClientFactory.Verify();
            returnedValue!.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        }

        [Test]
        public async Task Integration_Returns_Guid_When_Calling_AddTradePartyAsync()
        {
            // Arrange
            var tradeParty = new TradePartyDTO
            {
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            var guid = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(guid);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            var returnedValue = await _apiIntegration.AddTradePartyAsync(tradeParty);

            // Assert
            _mockHttpClientFactory.Verify();
            returnedValue!.Should().Be(guid);
        }

        [Test]
        public async Task API_Returns_200_When_Calling_UpdateTradePartyAsync()
        {
            // Arrange
            var tradeParty = new TradePartyDTO
            {
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            var guid = Guid.NewGuid();

            var jsonString = JsonConvert.SerializeObject(guid);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

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
            var tradeParty = new TradePartyDTO
            {
                Id = Guid.Empty,
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            var jsonString = JsonConvert.SerializeObject(Guid.Empty);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

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
            var tradeParty = new TradePartyDTO
            {
                Id = Guid.Empty,
                PartyName = "Trade party Ltd",
                NatureOfBusiness = "Wholesale Hamster Supplies",
                CountryName = "United Kingdom"
            };

            var jsonString = JsonConvert.SerializeObject(Guid.Empty);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            await Assert.ThrowsExceptionAsync<BadHttpRequestException>(async () => await _apiIntegration.UpdateTradePartyAsync(tradeParty));

            // Assert
            _mockHttpClientFactory.Verify();
        }

        [Test]
        public async Task Integration_Returns_LogisticsLocations_When_Calling_GetLogisticsLocationByPostcodeAsync()
        {
            // Arrange
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

            var jsonString = JsonConvert.SerializeObject(logisticsLocations);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            var returnedValue = await _apiIntegration.GetLogisticsLocationByPostcodeAsync(postcode);

            // Assert
            _mockHttpClientFactory.Verify();
            returnedValue!.Count.Should().Be(1);
            returnedValue[0].Name.Should().Be(logisticsLocations[0].Name);
            returnedValue[0].Address.LineOne.Should().Be(logisticsLocations[0].Address.LineOne);
            returnedValue[0].Address.LineTwo.Should().Be(logisticsLocations[0].Address.LineTwo);
            returnedValue[0].Address.CityName.Should().Be(logisticsLocations[0].Address.CityName);
            returnedValue[0].Address.PostCode.Should().Be(logisticsLocations[0].Address.PostCode);
        }

        [Test]
        public async Task Integration_Returns_RelationshipId_When_Calling_AddLogisticsLocationRelationship()
        {
            // Arrange
            var logisticsLocations = new LogisticsLocationRelationshipDTO
            {
                TraderId = Guid.NewGuid(),
                EstablishmentId = Guid.NewGuid()
            };

            var jsonString = JsonConvert.SerializeObject(logisticsLocations);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = httpContent
            };

            _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost/");

            _mockHttpClientFactory.Setup(x => x.CreateClient("Assurance")).Returns(httpClient).Verifiable();

            _apiIntegration = new ApiIntegration(_mockHttpClientFactory.Object);

            // Act
            var returnedValue = await _apiIntegration.AddLogisticsLocationRelationship(logisticsLocations);

            // Assert
            _mockHttpClientFactory.Verify();
            returnedValue!.Should().Be(logisticsLocations.EstablishmentId);
        }
    }
}
