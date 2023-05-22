﻿using Defra.Trade.ReMoS.AssuranceService.UI.Core.DTOs;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Integration;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Interfaces;
using Defra.Trade.ReMoS.AssuranceService.UI.Core.Services;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Integration
{
    internal class ApiIntegrationTests
    {
        private IAPIIntegration? _apiIntegration;
        protected Mock<IHttpClientFactory> _mockHttpClientFactory = new();
        protected Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

        [Test]
        public async Task API_Returns_200_When_Calling_GetAllTradePartiesAsync()
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
    }
}
